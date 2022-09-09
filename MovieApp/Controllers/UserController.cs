using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MovieApp.Db;
using MovieApp.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MovieApp.Controllers
{
    [Route("api/[Controller]/[Action]")]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public IConfiguration _Configuration { get; set; }   

        public UserController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _Configuration = configuration;
        }

        [HttpPost]
        public ActionResult Register([FromBody] Users users)
        {
            users.Password = BCrypt.Net.BCrypt.HashPassword(users.Password);
            _context.users.Add(users);
            _context.SaveChanges();
            return Created(new Uri($"{Request.Path}/{users.Id}", UriKind.Relative),users);
        }

        [HttpPost]
        public ActionResult Login([FromBody] Users users)
        {
            var checkuser = _context.users.FirstOrDefault(x => x.UserName == users.UserName);

            if (checkuser != null)
            {
                var verified = BCrypt.Net.BCrypt.Verify(users.Password, checkuser.Password);

                if (verified)
                {

                    var claims = new[] {
                    new Claim(JwtRegisteredClaimNames.Sub, _Configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id",  checkuser.Id.ToString()),
                        new Claim("UserName",checkuser.UserName),
                };
                    var secretkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_Configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken(
                        _Configuration["Jwt:Issuer"],
                        _Configuration["Jwt:Audience"],
                         claims,
                         expires: DateTime.UtcNow.AddMinutes(10),
                         signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("PLease Enter Correct Password");
                }

            }
            else
            {
                return BadRequest("Invalid Credentials");
            }

     
        }

    }
}
