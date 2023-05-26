using ChatServerWebApi.Models;
using MongoDB.Driver;

namespace ChatServerWebApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
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

        public IEnumerable<User> GetUsers()
        {
            return _users.Find(user => true).ToList();
        }

        //public User GetUserById(string userId)
        //{
        //    var partner = _users.Find(u => u.Id == userId).FirstOrDefault();
        //    return partner;
        //}
    }
}
