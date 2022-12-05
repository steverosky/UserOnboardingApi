using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserOnboardingApi.EFCore;
using UserOnboardingApi.Model;


namespace UserOnboardingApi.Controllers
{
    [Route("api/token")]
    [ApiController]
    public class ITokenController : ControllerBase
    {
        public IConfiguration _configuration;
        private readonly EF_DataContext _context;
        private readonly DbHelper _db;

        public ITokenController(IConfiguration config, EF_DataContext context, DbHelper db)
        {
            _configuration = config;
            _context = context;
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Post(userModel _userData)
        {
            if (_userData != null && _userData.Email != null && _userData.Password != null)
            {
                var user = await _db.GetUser(_userData.Email, _userData.Password);

                if (user != null)
                {
                    //create claims details based on the user information
                    var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["JwtConfig:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        //new Claim("Id", user.Id.ToString()),
                        new Claim("Email", user.Email.ToString()),
                        new Claim("Password", user.Password.ToString()),
                        };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtConfig:Secret"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["JwtConfig:Issuer"],
                        _configuration["JwtConfig:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }

    }
}
