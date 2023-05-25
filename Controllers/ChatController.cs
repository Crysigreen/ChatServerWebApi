using ChatServerWebApi.Hubs;
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

        public ChatController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
        {
            // Отправка сообщения через SignalR
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Content);

            return Ok();
        }
    }

    public class MessageDto
    {
        public string User { get; set; }
        public string Content { get; set; }
    }
}