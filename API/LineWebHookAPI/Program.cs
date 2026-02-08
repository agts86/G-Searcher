using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using LineDevSdk.Configurations;
using LineDevSdk.Https;
using LineWebHookAPI.Configurations;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Models.Job;
using LineWebHookAPI.Models.Services;
using Microsoft.EntityFrameworkCore;
using YahooDeveloperApiClient.Configuration;

namespace LineWebHookAPI;
[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        });
        builder.Services.AddApiVersioning().AddApiExplorer
        (
            options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            }
        );
        builder.Services.AddYOLPClient
        (
            options =>
            {
                options.AppId = builder.Configuration.GetValue<string>("Yahoo:AppId");
            }
        );
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();
        // 以下DI
        builder.Services.AddTransient<Middleware>();
        builder.Services.AddScoped<LineSignatureFilter>();
        builder.Services.AddScoped<IYahooRepository, YahooRepository>();
        builder.Services.AddScoped<IManagedRepository, ManagedRepository>();
        builder.Services.AddScoped<IMiddleWareRepository, MiddleWareRepository>();
        builder.Services.AddHttpClient<ILineMessagingClient, LineMessagingClient>();
        builder.Services.AddScoped<IYahooService, YahooService>();
        builder.Services.AddScoped<IManagedService, ManagedService>();
        builder.Services.AddSingleton<IBackgroundJobQueue<LocalJobDto>, BackgroundJobQueue<LocalJobDto>>();
        builder.Services.AddHostedService<YahooBackgroundService>();


        // Enumを文字列として扱う
        builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        // EFCoreの設定
        builder.Services.AddDbContext<LineWebHookContext>
        (
            options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection"))
        );

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "v1"));
        }
        app.MapGet("/health", () => Results.Ok("ok"));

        app.UseMiddleware<Middleware>();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
