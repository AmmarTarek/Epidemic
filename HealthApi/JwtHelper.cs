using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;



namespace HealthApi
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(User user, IConfiguration config)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            if (config == null) throw new ArgumentNullException(nameof(config));

            var key = config["Jwt:Key"];
            var issuer = config["Jwt:Issuer"];

            if (string.IsNullOrEmpty(key)) throw new Exception("JWT Key is missing in config.");
            if (string.IsNullOrEmpty(issuer)) throw new Exception("JWT Issuer is missing in config.");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FirstName + " " + user.LastName),
            new Claim("RoleId", user.RoleId.ToString())
        };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
