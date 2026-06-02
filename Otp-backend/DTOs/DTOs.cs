namespace Otp.DTOs;

// ── Auth ─────────────────────────────────────────────────────────────────────

public record LoginRequest(string Username, string Password);

public record LoginResponse(bool Success, string UserId, string? Message = null);

// ── OTP ──────────────────────────────────────────────────────────────────────

public record GenerateOtpRequest(string UserId);

public record GenerateOtpResponse(bool Success, string Otp, int ExpiresInSeconds, string? Message = null);

public record VerifyOtpRequest(string UserId, string Otp);

public record VerifyOtpResponse(bool Success, string? Message = null);
