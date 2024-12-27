using CMSDemoAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMSDemoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly BookDbContext _context;
        private readonly SymmetricSecurityKey _key;

        public TokenController(BookDbContext context, IConfiguration config)
        {
            _context = context;
            _key = new SymmetricSecurityKey(UTF8Encoding.UTF8.GetBytes(config["Key"]));

        }
        [HttpPost]
        public IActionResult GenerateToken(User luser)
        {
            string token = string.Empty;
            // Retrieve the full user details from the database
            var user = _context.Users.FirstOrDefault(u => u.email == luser.email && u.password == luser.password && u.role==luser.role);
            if (ValidateUser(user.email, user.password,user.role))
            {
                var claims = new List<Claim>
               {
                   new Claim(JwtRegisteredClaimNames.NameId,user.userName!),
                   new Claim(JwtRegisteredClaimNames.Email,user.email),
                   new Claim(ClaimTypes.Role,user.role)//Add the user role as a claim

               };
                var cred = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
                var tokenDescription = new SecurityTokenDescriptor
                {
                    SigningCredentials = cred,
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddMinutes(5)
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                var createToken = tokenHandler.CreateToken(tokenDescription);
                token = tokenHandler.WriteToken(createToken);
                return Ok(new { token, role = user.role });
            }
            else
            {
                return BadRequest(new { message = "Invalid email or password" });
            }
        }
        private bool ValidateUser(string email, string password,string requiredrole)
        {
            var users = _context.Users.ToList();
            var user = users.FirstOrDefault(u => u.email == email && u.password == password);
            return user!=null && user.role == requiredrole;
            //if (user != null)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
        }


    }
}
