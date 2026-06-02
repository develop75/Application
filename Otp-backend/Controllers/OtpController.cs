using Otp.DTOs;
using Otp.Services;
using Microsoft.AspNetCore.Mvc;

namespace Otp.Controllers;

[ApiController]
[Route("api/otp")]
public class OtpController(IOtpService otpService) : ControllerBase
{
    /// <summary>
    /// Genera un nuovo OTP per l'utente. Invalida automaticamente i codici precedenti.
    /// Chiamato dall'app mobile React Native.
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(GenerateOtpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GenerateOtpResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Generate([FromBody] GenerateOtpRequest request)
    {
        if (!int.TryParse(request.UserId, out var userId) || userId <= 0)
            return BadRequest(new GenerateOtpResponse(false, string.Empty, 0, "UserId non valido."));

        var (code, expiresInSeconds) = await otpService.GenerateAsync(userId);

        return Ok(new GenerateOtpResponse(true, code, expiresInSeconds));
    }

    /// <summary>
    /// Verifica un OTP inserito dall'utente nella web app.
    /// Il codice è monouso: dopo la verifica viene marcato come utilizzato.
    /// Chiamato dalla web app.
    /// </summary>
    [HttpPost("verify")]
    [ProducesResponseType(typeof(VerifyOtpResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(VerifyOtpResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Verify([FromBody] VerifyOtpRequest request)
    {
        if (!int.TryParse(request.UserId, out var userId) || userId <= 0)
            return BadRequest(new VerifyOtpResponse(false, "UserId non valido."));

        if (string.IsNullOrWhiteSpace(request.Otp))
            return BadRequest(new VerifyOtpResponse(false, "Codice OTP mancante."));

        var valid = await otpService.VerifyAsync(userId, request.Otp.Trim());

        if (!valid)
            return BadRequest(new VerifyOtpResponse(false, "Codice non valido o scaduto."));

        return Ok(new VerifyOtpResponse(true));
    }
}
