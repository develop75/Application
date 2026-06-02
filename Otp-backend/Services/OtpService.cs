using Otp.Data;
using Otp.Models;
using Microsoft.EntityFrameworkCore;

namespace Otp.Services;

public interface IOtpService
{
    Task<(string Code, int ExpiresInSeconds)> GenerateAsync(int userId);
    Task<bool> VerifyAsync(int userId, string code);
}

public class OtpService(AppDbContext db, IConfiguration config) : IOtpService
{
    private readonly int _expiryMinutes = int.Parse(config["Otp:ExpiryMinutes"] ?? "5");
    private readonly int _length = int.Parse(config["Otp:Length"] ?? "6");

    public async Task<(string Code, int ExpiresInSeconds)> GenerateAsync(int userId)
    {
        // Invalida tutti gli OTP precedenti ancora validi per questo utente
        var existing = await db.OtpCodes
            .Where(o => o.UserId == userId && !o.Used && o.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        foreach (var old in existing)
            old.Used = true;

        // Genera nuovo codice numerico
        var code = GenerateNumericCode(_length);

        var otp = new OtpCode
        {
            UserId = userId,
            Code = code,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_expiryMinutes),
            Used = false
        };

        db.OtpCodes.Add(otp);
        await db.SaveChangesAsync();

        return (code, _expiryMinutes * 60);
    }

    public async Task<bool> VerifyAsync(int userId, string code)
    {
        var otp = await db.OtpCodes
            .Where(o =>
                o.UserId == userId &&
                o.Code == code &&
                !o.Used &&
                o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otp is null)
            return false;

        // Segna come usato (OTP monouso)
        otp.Used = true;
        await db.SaveChangesAsync();

        return true;
    }

    // Genera un codice numerico crittograficamente sicuro
    private string GenerateNumericCode(int length)
    {
        var digits = new char[length];
        var bytes = System.Security.Cryptography.RandomNumberGenerator.GetBytes(length);
        for (int i = 0; i < length; i++)
            digits[i] = (char)('0' + bytes[i] % 10);
        return new string(digits);
    }
}
