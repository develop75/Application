using Otp.Data;
using Otp.DTOs;
using Otp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Otp.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(AppDbContext db, IJwtService jwt) : ControllerBase
{
    /// <summary>
    /// Login utente. Restituisce un userId (stringa numerica) da passare alle chiamate OTP.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new LoginResponse(false, string.Empty, "Username e password sono obbligatori."));

        var user = await db.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username.Trim() && u.IsActive);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new LoginResponse(false, string.Empty, "Credenziali non valide."));

        // userId restituito come stringa per compatibilità con l'app React Native
        var userId = user.Id.ToString();

        return Ok(new LoginResponse(true, userId));
    }
}
