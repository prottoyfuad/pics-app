
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebAppController.Dtos;
using WebAppController.Interfaces;
using WebAppController.Models;

namespace WebAppController.Controllers
{
    [Route("[controller]")]
    public class AuthController : ControllerBase {
        private IDataLoader data;
        private IConfiguration config;
        public AuthController(IConfiguration configuration, IDataLoader dataLoader) {
            this.data = dataLoader;
            this.config = configuration;
        }

        [HttpPost("register")]
        public async Task<ActionResult<TokenDto>> Register([FromBody] LoginDto info) {
            using var hmac = new HMACSHA512();
            User usr = new User {
                UserName = info.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(info.Password)),
                PasswordSalt = hmac.Key
            };
            bool added = await data.AddUserAsync(usr);
            if (!added) {
                return Unauthorized("Username Taken!");
            }
            return Ok(new TokenDto {
                Username = usr.UserName,
                Token = CreateToken(usr)
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto info) {
            var usr = await data.GetUserAsync(info.Username.ToLower());
            if (usr == null) {
                return Unauthorized("Invalid Username");
            }
            using var hmac = new HMACSHA512(usr.PasswordSalt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(info.Password));
            if (BitConverter.ToString(hash) != BitConverter.ToString(usr.PasswordHash)) {
                return Unauthorized("Invalid Password");
            }
            return Ok(new TokenDto {
                Username = usr.UserName,
                Token = CreateToken(usr)
            });
        }

        private string CreateToken(User user) {
            var secretKey = config["Jwt:Key"];
            var tokenIssuer = config["Jwt:Issuer"];
            var tokenAudience = config["Jwt:Audience"];
            if (secretKey == null || tokenIssuer == null || tokenAudience == null) {
                throw new ArgumentNullException("All token configs not found.");
            }
            if (secretKey.Length < 64) {
                throw new Exception("Secret Key needs to be longer.");
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var token = new JwtSecurityToken(
                issuer: tokenIssuer,
                audience: tokenAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature)
            );
            return (new JwtSecurityTokenHandler()).WriteToken(token);
        }
    }
}
