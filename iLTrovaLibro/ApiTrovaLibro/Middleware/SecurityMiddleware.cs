namespace ApiTrovaLibro.Middleware
{
    using ApiTrovaLibro.Models;
    using global::Helper.Log;
    using Microsoft.Extensions.Options;
    using System.Net.Http;
    using System.Text.Json;

    /// <summary>
    /// Middleware per la sicurezza delle API, gestisce controlli sugli header, ovvero blocca chiamate da tool come postman e reCAPTCHA.
    /// Bisogna immettere l'header personalizzato X-App-Identify-Token con il valore segreto per autorizzare le richieste.
    /// </summary>
    public class SecurityMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly string _customHeaderName = "X-App-Identify-Token";
        //private readonly string _expectedHeaderValue = "IlTuoCodiceSegretoPrivato"; // Da mettere in appsettings.json

        public SecurityMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ILog log)
        {
            // Recupera l'IP
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();

            log.Info($"Richiesta ricevuta da IP: {ipAddress}");

            // 1. Controllo Header Personalizzato (Blocca Postman base e siti esterni)
            if (!context.Request.Headers.TryGetValue(AppSettings.CustomHeaderName, out var extractedHeader) ||
                extractedHeader != AppSettings.ExpectedHeaderValue)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsync("Accesso negato: Sorgente non autorizzata.");
                return;
            }

            // 2. Controllo reCAPTCHA (Solo per rotte sensibili come /User/Create)
            if (context.Request.Path.StartsWithSegments("/api/User/Create") && context.Request.Method == "POST")
            {
                var captchaToken = context.Request.Headers["X-Recaptcha-Token"].ToString();

                if (string.IsNullOrEmpty(captchaToken) || !await ValidateRecaptcha(captchaToken))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Validazione umana fallita.");
                    return;
                }
            }

            await _next(context);
        }

        private async Task<bool> ValidateRecaptcha(string token)
        {
            // Qui andrà la chiamata HTTP ai server di Google
            // Per ora restituiamo true, ma qui va implementata la logica HttpClient
            return true;
        }
    }
}
