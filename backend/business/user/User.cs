using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using backend.config;
using backend.dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backend.business.user
{
    public class User : IUser
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _iconfiguration;
        private readonly ILogger<User> _logger;

        public User(AppDbContext context, IConfiguration configuration, ILogger<User> logger)
        {
            _context = context;
            _iconfiguration = configuration;
            _logger = logger;
        }

        public async Task<string> Authenticate(AuthDTO auth)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == auth.Username && u.Password == auth.Password);
                if (user == null) return null;

                var claimns = new[] { new Claim(ClaimTypes.Name, user.Username) };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfiguration["JWT:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _iconfiguration["JWT:Issuer"],
                    audience: _iconfiguration["JWT:Audience"],
                    claims: claimns,
                    expires: DateTime.Now.AddHours(double.Parse(_iconfiguration["JWT:TimeHora"])),
                    signingCredentials: creds
                );

                user.LasDateEntry = DateTime.Now;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autenticar usuario {username}", auth.Username);
                throw;
            }
        }

        public async Task<UserDTO> GetUser(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var claims = handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_iconfiguration["JWT:Key"])),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var username = claims.Identity?.Name;

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null) return null;

                return new UserDTO
                {
                    Codigo = user.Codigo,
                    Name = user.Name,
                    Username = user.Username,
                    LasDateEntry = user.LasDateEntry
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario: {message}", ex.Message);
                throw;
            }
        }
    }
}
