# Citterio OTP — Backend .NET 8

API REST in ASP.NET Core 8 per la generazione e verifica di codici OTP, con MS SQL Server e Entity Framework Core.

---

## Struttura del progetto

```
citterio-otp-backend/
├── Controllers/
│   ├── AuthController.cs       # POST /api/auth/login
│   └── OtpController.cs        # POST /api/otp/generate  |  POST /api/otp/verify
├── Data/
│   └── AppDbContext.cs         # EF Core DbContext
├── DTOs/
│   └── DTOs.cs                 # Request/Response record
├── Models/
│   └── Models.cs               # Entità User e OtpCode
├── Services/
│   ├── JwtService.cs           # Generazione token JWT
│   └── OtpService.cs           # Logica OTP
├── Program.cs                  # Startup, DI, middleware
├── appsettings.json            # Configurazione
├── database.sql                # Script SQL alternativo alle migration
└── CitterioOtp.csproj
```

---

## Setup

### 1. Prerequisiti

- .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8
- SQL Server (qualsiasi edizione, anche Express o Developer)

### 2. Configura la connessione al database

Apri `appsettings.json` e modifica la connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=NomeServer;Database=CitterioOtp;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Esempi comuni:
- Istanza locale default: `Server=.;Database=CitterioOtp;...`
- Istanza named: `Server=.\SQLEXPRESS;Database=CitterioOtp;...`
- Con credenziali SQL: `Server=.;Database=CitterioOtp;User Id=sa;Password=tuapassword;TrustServerCertificate=True;`

### 3. Cambia la chiave JWT

In `appsettings.json`, sostituisci il valore di `Jwt:Key` con una stringa lunga e casuale (minimo 32 caratteri):

```json
"Jwt": {
  "Key": "INSERISCI_QUI_UNA_CHIAVE_SEGRETA_LUNGA_E_SICURA"
}
```

### 4. Crea il database

**Opzione A — Migration EF Core (consigliata)**

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate
dotnet ef database update
```

In modalità Development, le migration vengono applicate automaticamente all'avvio.

**Opzione B — Script SQL manuale**

Esegui `database.sql` direttamente su SQL Server Management Studio o sqlcmd.

### 5. Avvia il backend

```bash
dotnet run
```

Il server si avvia su `http://localhost:5000` (HTTP) e `https://localhost:5001` (HTTPS).

Swagger UI disponibile in sviluppo: `http://localhost:5000/swagger`

---

## Endpoint API

### POST `/api/auth/login`

Autentica l'utente e restituisce il suo `userId`.

**Request:**
```json
{ "username": "admin", "password": "Citterio2024!" }
```

**Response 200:**
```json
{ "success": true, "userId": "1" }
```

**Response 401:**
```json
{ "success": false, "userId": "", "message": "Credenziali non valide." }
```

---

### POST `/api/otp/generate`

Genera un nuovo OTP per l'utente. Invalida automaticamente gli OTP precedenti.
Chiamato dall'**app mobile**.

**Request:**
```json
{ "userId": "1" }
```

**Response 200:**
```json
{ "success": true, "otp": "847392", "expiresInSeconds": 300 }
```

---

### POST `/api/otp/verify`

Verifica il codice inserito nella web app. Il codice è monouso.
Chiamato dalla **web app**.

**Request:**
```json
{ "userId": "1", "otp": "847392" }
```

**Response 200:**
```json
{ "success": true }
```

**Response 400:**
```json
{ "success": false, "message": "Codice non valido o scaduto." }
```

---

## Configurazione OTP

In `appsettings.json`:

```json
"Otp": {
  "ExpiryMinutes": 5,   // Durata validità OTP in minuti
  "Length": 6            // Numero di cifre del codice
}
```

---

## Gestione utenti

Gli utenti si gestiscono direttamente su MS SQL. Per creare un nuovo utente, genera prima l'hash BCrypt della password con questo snippet C#:

```csharp
var hash = BCrypt.Net.BCrypt.HashPassword("NuovaPassword123!");
Console.WriteLine(hash);
```

Poi inserisci il record:

```sql
INSERT INTO Users (Username, PasswordHash, DisplayName, IsActive, CreatedAt)
VALUES ('mario.rossi', '<hash_generato>', 'Mario Rossi', 1, GETUTCDATE());
```

---

## Configurazione CORS

In `Program.cs`, la policy `AppPolicy` accetta richieste da qualsiasi origine.
In produzione, limitala alla sola web app:

```csharp
policy.WithOrigins("https://tua-webapp.citterio.it")
```

---

## Credenziali default

| Campo    | Valore         |
|----------|----------------|
| Username | `admin`        |
| Password | `Citterio2024!`|

Cambia la password subito dopo il primo avvio.
