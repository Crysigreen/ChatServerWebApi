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

        public UsersController(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(User user)
        {
            // Проверка на уникальность имени пользователя или другие проверки

            // Регистрация пользователя
            var registeredUser = _userService.Register(user);
            return registeredUser;
        }

        [HttpPost("login")]
        public ActionResult<User> Login(User loginRequest)
        {
            // Проверка имени пользователя и пароля

            // Аутентификация пользователя
            var authenticatedUser = _userService.Login(loginRequest.Username, loginRequest.Password);
            return authenticatedUser;
        }

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
