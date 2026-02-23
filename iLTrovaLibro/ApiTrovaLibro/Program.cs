
using ApiTrovaLibro.Models;
using Helper.Log;
using Microsoft.AspNetCore.Diagnostics;
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
            builder.Services.AddScoped<IRepository, CategoryFactory>();

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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
