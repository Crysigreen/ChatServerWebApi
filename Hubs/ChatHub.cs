using ChatServerWebApi.Models;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

public class ChatHub : Hub
{
    private readonly IMongoCollection<ChatMessage> _chatMessageCollection;

    public ChatHub(IMongoDatabase database)
    {
        _chatMessageCollection = database.GetCollection<ChatMessage>("ChatMessages");
    }

    public async Task SendMessage(string senderId, string receiverId, string content)
    {
        var message = new ChatMessage
        {
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        // Сохраните сообщение в базе данных
        await _chatMessageCollection.InsertOneAsync(message);

        // Отправьте сообщение всем подключенным клиентам
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}