using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ChatServerWebApi.Models;
using System.Collections.Generic;
using ChatServerWebApi.Services;

namespace ChatServerWebApi.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public UsersController(UserService userService, AuthService authService)
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(User user)
        {
            // Проверка на уникальность имени пользователя или другие проверки

            // Регистрация пользователя
            var registeredUser = _userService.Register(user);
            return registeredUser;
        }

        //[HttpPost("login")]
        //public ActionResult<User> Login(User loginRequest)
        //{
        //    // Проверка имени пользователя и пароля

        //    // Аутентификация пользователя
        //    var authenticatedUser = _userService.Login(loginRequest.Username, loginRequest.Password);
        //    return authenticatedUser;
        //}

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            var token = _authService.Authenticate(username, password);

            if (token == null)
            {
                return Unauthorized();
            }

            return Ok(new { token });
        }

        [HttpGet("getAllUsers")]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            var users = _userService.GetUsers();
            var userDtos = users.Select(u => new UserDto { Id = u.Id, Username = u.Username });
            return Ok(userDtos);
        }

        //[HttpGet("{userId}")]
        //public ActionResult<User> GetUserById(string userId)
        //{
        //    var user = _userService.GetUserById(userId);
        //    if (user == null)
        //    {
        //        return NotFound();
        //    }
        //    var userDto = user.Select
        //    return Ok(user.Username);
        //}

        //[HttpPost("login")]
        //public ActionResult<User> Login(string Username, string Password)
        //{
        //    // Проверка имени пользователя и пароля

        //    // Аутентификация пользователя
        //    var authenticatedUser = _userService.Login(Username, Password);
        //    return authenticatedUser;
        //}
    }
}
