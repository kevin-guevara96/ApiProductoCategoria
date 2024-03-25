using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataAccess;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace APIPRUEBAS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly string secretKey;
        private readonly DBAPIContext _context;

        public AutenticacionController(IConfiguration config, DBAPIContext context)
        {
            secretKey = config.GetSection("settings").GetSection("secretkey").ToString();
            _context = context;
        }

        [HttpPost, Route("Validar")]
        public IActionResult Validar([FromBody] Usuario r)
        {
            var encontrar = _context.Usuarios.Where(x => x.Nombre == r.Nombre && x.Clave == r.Clave).FirstOrDefault();

            if (encontrar is not null)
            {
                var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                var claims = new ClaimsIdentity();

                claims.AddClaim(new Claim(ClaimTypes.NameIdentifier,r.Nombre));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claims,
                    Expires = DateTime.UtcNow.AddMinutes(5),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes),SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                string tokenCreado = tokenHandler.WriteToken(tokenConfig);

                return StatusCode(StatusCodes.Status200OK, new { token = tokenCreado });
            }

            return StatusCode(StatusCodes.Status401Unauthorized, new { token = "" });
        }
    }
}
