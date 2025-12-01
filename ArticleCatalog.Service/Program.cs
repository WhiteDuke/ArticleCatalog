using System;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using ArticleCatalog.DataAccess;
using ArticleCatalog.Domain.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;

namespace ArticleCatalog.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Is(LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(
                restrictedToMinimumLevel: LogEventLevel.Warning,
                outputTemplate: "{Timestamp: dd.MM.yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,
                restrictedToMinimumLevel: LogEventLevel.Warning,
                fileSizeLimitBytes: 10485760,
                retainedFileCountLimit: 31,
                outputTemplate: "{Timestamp:dd.MM.yyyy HH:mm:ss} [{Level}] {Message}{NewLine}{Exception}")
            .CreateLogger();

        builder.Host.UseSerilog();

        // Add services to the container.
        builder.Services.AddDbContext<ArticleCatalogDbContext>(options =>
        {
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSqlConnection"), optionsBuilder =>
            {
                optionsBuilder.MigrationsAssembly(typeof(ArticleCatalogDbContext).Assembly.FullName);
            });
        });

        builder.Services.AddScoped<IArticleCatalogService, ArticleCatalogService>();
        builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo()
            {
                Title = "Каталог статей",
                Version = "v1",
                Description = "HTTP API для работы с каталогом статей"
            });
        });

        var app = builder.Build();

        app.UseCors(policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        var isDatabaseReady = false;
        var retryCount = 0;

        while (!isDatabaseReady && retryCount < 10)
        {
            try
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ArticleCatalogDbContext>();
                dbContext.Database.Migrate();
                isDatabaseReady = true;
            }
            catch (Exception ex)
            {
                retryCount++;
                Thread.Sleep(2000);
                Log.Error(ex,"Попытка подключения к БД №{RetryCount}. Ошибка: {ExMessage}", retryCount, ex.Message);
            }
        }

        app.Run();
    }
}