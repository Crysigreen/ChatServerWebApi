using ChatServerWebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ChatServerWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly UserService _userService;

        public ChatController(IHubContext<ChatHub> hubContext, UserService userService)
        {
            _hubContext = hubContext;
            _userService = userService;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
        {
            // Отправка сообщения через SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Content);

            return Ok();
        }

        [HttpGet("GetChatHistory")]
        public async Task<IActionResult> GetChatHistory(string senderUsername,string friendUsername, int pageIndex, int pageSize)
        {
            /*var username = User.Identity.Name;*/  // Получаем имя текущего пользователя
            var messages = await _userService.GetChatHistory(senderUsername, friendUsername, pageIndex, pageSize);
            return Ok(messages);
        }

    }

    public class MessageDto
    {
        public string User { get; set; }
        public string Content { get; set; }
    }
}