
using ApiTrovaLibro.Middleware;
using ApiTrovaLibro.Models;
using Helper.Log;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;
using TrovaLibro.DataContext;
using TrovaLibroLib.Dto;
using TrovaLibroLib.Factory;

namespace ApiTrovaLibro
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var conf = builder.Configuration;

            // Add services to the container.
            builder.Configuration.GetSection("AppSettings").Get<AppSettings>();

            var permitLimitForHour = builder.Configuration.GetValue<int>("AppSettings:PermitLimitForHour");
            var maxRecords = builder.Configuration.GetValue<long>("AppSettings:MaxRecordsLoaded");
            var allowedOriginsString = builder.Configuration.GetValue<string>("AppSettings:AllowedOrigins") ?? "";
            var allowedOrigins = allowedOriginsString.Split(',', StringSplitOptions.RemoveEmptyEntries);

            builder.Services.AddRateLimiter(options => {
                options.AddFixedWindowLimiter(policyName: "signup-limit", opt => {
                    opt.Window = TimeSpan.FromHours(1);     // Finestra temporale
                    opt.PermitLimit = permitLimitForHour;   // Max 3 richieste ogni ora
                    opt.QueueLimit = 0;                     // Nessuna coda, rifiuta subito
                });
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "MyAllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });

            // 1. Aggiungi il DbContext (SQL Server in questo esempio)
            builder.Services.AddDbContext<DbTrovaLibroContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<LoggerOptions>(conf.GetSection("LoggerOptions"));
            builder.Services.AddScoped<ILog, Logger>(op =>
            {
                var obj = op.GetService<IOptionsMonitor<LoggerOptions>>().CurrentValue;
                return new Logger(obj);
            });

            // 2. Registra il Repository (Dependency Injection)
            // Usiamo 'AddScoped' affinché venga creata un'istanza per ogni richiesta HTTP
            builder.Services.AddScoped<CategoryFactory>();
            builder.Services.AddScoped<BookFactory>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(options => {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            var app = builder.Build();

            /// Configurazione del middleware per la gestione globale delle eccezioni
            app.UseExceptionHandler(exceptionHandlerApp =>
            {
                exceptionHandlerApp.Run(async context =>
                {
                    // RECUPERO DEL LOG: Ottieni l'istanza di ILog registrata nello scope della richiesta
                    var log = context.RequestServices.GetRequiredService<ILog>();

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    var exception = exceptionHandlerPathFeature?.Error;

                    // Logga l'errore usando il sistema di logging
                    log.Error($"{exception.Message}:\n{exception.StackTrace}"); 

                    var response = new InfoError
                    {
                        Title = "Si è verificato un errore interno",
                        Status = 500,
                        Detail = exception?.Message // In produzione, meglio un messaggio generico
                    };

                    await context.Response.WriteAsJsonAsync(response);
                });
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //**********************************************
            // Attiva il tuo buttafuori personalizzato
            app.UseMiddleware<SecurityMiddleware>();
            //**********************************************


            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();


            //Se la tua WebApp è ospitata dietro un Reverse Proxy(come Nginx, IIS o Azure Load Balancer), 
            //l'indirizzo IP che il server vede potrebbe essere sempre lo stesso (quello del proxy). 
            //In questo caso, devi assicurarti di aver configurato i Forwarded Headers
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseStaticFiles();

            // Abilita CORS per permettere al client Next.js di chiamare l'API
            app.UseCors("MyAllowSpecificOrigins");

            app.Run();
        }
    }
}
