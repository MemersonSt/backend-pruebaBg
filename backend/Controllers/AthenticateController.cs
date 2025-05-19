using backend.business.user;
using backend.dto;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AthenticateController : ControllerBase
    {
        private readonly IUser _user;

        public AthenticateController(IUser user)
        {
            _user = user;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate([FromBody] AuthDTO auth)
        {
            var token = await _user.Authenticate(auth);
            if (token == null) return NotFound("Usuaurio no Encontrado"); 

            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Ok();
        }

        [HttpGet("session")]
        public async Task<IActionResult> GetUser()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token)) return Unauthorized();

            var data = await _user.GetUser(token);
            if (data == null) return Unauthorized();

            return Ok(data);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Path = "/",
                SameSite = SameSiteMode.None
            });
            return Ok();
        }

        [HttpGet("check")]
        public IActionResult Check()
        {
            var token = Request.Cookies["token"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }
            return Ok();
        }
    }
}
