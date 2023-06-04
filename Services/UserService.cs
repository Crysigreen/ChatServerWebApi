using ChatServerWebApi.Models;
using MongoDB.Driver;

namespace ChatServerWebApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<ChatMessage> _messagesCollection;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
            _messagesCollection = database.GetCollection<ChatMessage>("ChatMessages");
        }

        public User Register(User user)
        {
            // Добавление пользователя в базу данных
            _users.InsertOne(user);
            return user;
        }

        public User Login(string username, string password)
        {
            // Поиск пользователя по имени пользователя и паролю
            var user = _users.Find(u => u.Username == username && u.Password == password).FirstOrDefault();
            return user;
        }

        public User LoginJWT(string username)
        {
            // Поиск пользователя по имени пользователя и паролю
            var user = _users.Find(u => u.Username == username).FirstOrDefault();
            return user;
        }


        public IEnumerable<User> GetUsers()
        {
            return _users.Find(user => true).ToList();
        }

        public async Task<List<ChatMessage>> GetChatHistory(string userUsername, string friendUsername, int pageIndex, int pageSize)
        {
            var filter = Builders<ChatMessage>.Filter.Where(m =>
                (m.From == userUsername && m.To == friendUsername) ||
                (m.From == friendUsername && m.To == userUsername));

            var messages = await _messagesCollection.Find(filter)
                .Skip(pageSize * pageIndex)
                .Limit(pageSize)
                .SortByDescending(m => m.Timestamp)
                .ToListAsync();

            return messages;
        }
    }
}
