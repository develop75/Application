-- ============================================================
--  Citterio OTP — Script creazione database MS SQL
--  Esegui questo script se preferisci non usare le migration EF
-- ============================================================

USE master;
GO

-- Crea il database se non esiste
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'CitterioOtp')
BEGIN
    CREATE DATABASE Otp;
END
GO

USE CitterioOtp;
GO

-- ── Tabella Users ─────────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        Username    NVARCHAR(100)   NOT NULL,
        PasswordHash NVARCHAR(256)  NOT NULL,
        DisplayName NVARCHAR(200)   NOT NULL DEFAULT '',
        IsActive    BIT             NOT NULL DEFAULT 1,
        CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),

        CONSTRAINT UQ_Users_Username UNIQUE (Username)
    );

    -- Indice per ricerca rapida per username
    CREATE INDEX IX_Users_Username ON Users (Username);
END
GO

-- ── Tabella OtpCodes ──────────────────────────────────────
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OtpCodes')
BEGIN
    CREATE TABLE OtpCodes (
        Id          INT IDENTITY(1,1) PRIMARY KEY,
        UserId      INT             NOT NULL,
        Code        NCHAR(6)        NOT NULL,
        CreatedAt   DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        ExpiresAt   DATETIME2       NOT NULL,
        Used        BIT             NOT NULL DEFAULT 0,

        CONSTRAINT FK_OtpCodes_Users FOREIGN KEY (UserId)
            REFERENCES Users(Id) ON DELETE CASCADE
    );

    CREATE INDEX IX_OtpCodes_UserId ON OtpCodes (UserId);
END
GO

-- ── Utente admin di default ───────────────────────────────
-- Password: Citterio2024!
-- Hash BCrypt generato con BCrypt.Net: BCrypt.HashPassword("Citterio2024!")
-- IMPORTANTE: cambia la password al primo accesso
IF NOT EXISTS (SELECT * FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users (Username, PasswordHash, DisplayName, IsActive, CreatedAt)
    VALUES (
        'admin',
        '$2a$11$K5Ld0fFZfLH1X5u5T8mX9.2z7jQv3uK1wYp8nMcR6sA4dVbE0LjAi',
        'Amministratore',
        1,
        GETUTCDATE()
    );
END
GO

-- ── Pulizia automatica OTP scaduti (opzionale) ────────────
-- Crea questo job SQL Agent per pulire i vecchi OTP ogni notte
-- oppure eseguilo manualmente periodicamente:
--
-- DELETE FROM OtpCodes
-- WHERE ExpiresAt < DATEADD(day, -7, GETUTCDATE());

PRINT 'Database CitterioOtp creato con successo.';
GO
