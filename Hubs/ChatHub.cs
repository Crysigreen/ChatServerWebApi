using ChatServerWebApi.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
    private readonly IMongoCollection<Message> _messages;

    private static ConcurrentDictionary<string, string> clients = new ConcurrentDictionary<string, string>();

    public ChatHub(IMongoDatabase database)
    {
        _messages = database.GetCollection<Message>("ChatMessages");
    }

    public override async Task OnConnectedAsync()
    {
        // Когда клиент подключается, получаем его имя пользователя из строки запроса и сохраняем его в отображении.
        var httpContext = Context.GetHttpContext();
        var username = httpContext.Request.Query["username"].ToString();
        clients.TryAdd(Context.ConnectionId, username);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // Когда клиент отключается, удаляем его из отображения.
        clients.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(/*string senderId, string receiverId, string content,*/string receiverUsername, string message)
    {

        // Получение имени отправителя по его ConnectionId.
        var senderConnectionId = Context.ConnectionId;
        var senderUsername = clients[senderConnectionId];

        // Получение ConnectionId получателя по его имени пользователя.
        var receiverConnectionId = clients.FirstOrDefault(x => x.Value == receiverUsername).Key;

        if (!string.IsNullOrEmpty(receiverConnectionId))
        {
            // Отправка сообщения получателю.
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", senderUsername, message);

            // Создание объекта сообщения и сохранение его в MongoDB.
            var chatMessage = new Message { Sender = senderUsername, Receiver = receiverUsername, Content = message, Timestamp = DateTime.UtcNow };
            await _messages.InsertOneAsync(chatMessage);
        }

        //var message = new ChatMessage
        //{
        //    SenderId = senderId,
        //    ReceiverId = receiverId,
        //    Content = content,
        //    Timestamp = DateTime.UtcNow
        //};

        //// Сохраните сообщение в базе данных
        //await _chatMessageCollection.InsertOneAsync(message);

        //// Отправьте сообщение всем подключенным клиентам
        //await Clients.All.SendAsync("ReceiveMessage", message);
    }
}

public class Message
{
    // Идентификатор сообщения в MongoDB.
    public ObjectId Id { get; set; }

    // Имя пользователя-отправителя.
    public string Sender { get; set; }

    // Имя пользователя-получателя.
    public string Receiver { get; set; }

    // Текст сообщения.
    public string Content { get; set; }

    // Время отправки сообщения.
    public DateTime Timestamp { get; set; }
}