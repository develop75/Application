using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Otp.Services;

public interface IJwtService
{
    string GenerateToken(int userId, string username);
    ClaimsPrincipal? ValidateToken(string token);
}

public class JwtService(IConfiguration config) : IJwtService
{
    private readonly string _key = config["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT Key non configurata in appsettings.json");
    private readonly string _issuer = config["Jwt:Issuer"] ?? "CitterioOtp";
    private readonly string _audience = config["Jwt:Audience"] ?? "CitterioOtpApp";
    private readonly int _expiryMinutes = int.Parse(config["Jwt:ExpiryMinutes"] ?? "480");

    public string GenerateToken(int userId, string username)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            return handler.ValidateToken(token, parameters, out _);
        }
        catch
        {
            return null;
        }
    }
}
