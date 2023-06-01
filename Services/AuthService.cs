using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatServerWebApi.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly string _jwtKey;

        public AuthService(UserService userService, IConfiguration config)
        {
            _userService = userService;
            _jwtKey = config.GetSection("JwtKey").Value;
        }

        public string Authenticate(string username, string password)
        {
            var user = _userService.LoginJWT(username);

            if (user == null || user.Password != password)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
