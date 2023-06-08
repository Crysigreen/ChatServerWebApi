using ChatServerWebApi.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;

public class ChatHub : Hub
{
    private readonly IMongoCollection<ChatMessage> _messages;

    private static ConcurrentDictionary<string, string> clients = new ConcurrentDictionary<string, string>();

    public ChatHub(IMongoDatabase database)
    {
        _messages = database.GetCollection<ChatMessage>("ChatMessages");
    }

    public override async Task OnConnectedAsync()
    {
        // Когда клиент подключается, получаем его имя пользователя из строки запроса и сохраняем его в отображении.
        //var httpContext = Context.GetHttpContext();
        var username = Context.GetHttpContext().Request.Query["username"].ToString();
        clients.TryAdd(Context.ConnectionId, username);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // When the client is disconnected, remove it from display.
        clients.TryRemove(Context.ConnectionId, out _);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(string receiverUsername, string message)
    {

        // Get the sender's name by its ConnectionId.
        var senderConnectionId = Context.ConnectionId;
        var senderUsername = clients[senderConnectionId];

        // Get the receiver ConnectionId from his user name.
        var receiverConnectionId = clients.FirstOrDefault(x => x.Value == receiverUsername).Key;

        var chatMessage = new ChatMessage
        {
            From = senderUsername,
            To = receiverUsername,
            Content = message,
            Timestamp = DateTime.UtcNow
        };

        await _messages.InsertOneAsync(chatMessage);

        if (!string.IsNullOrEmpty(receiverConnectionId))
        {
            // Sending a message to the receiver.
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", chatMessage);

            // Создание объекта сообщения и сохранение его в MongoDB.
            //await _messages.InsertOneAsync(chatMessage);
        }

    }
}
