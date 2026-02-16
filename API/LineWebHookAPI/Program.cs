using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using LineDevSdk.Configurations;
using LineDevSdk.Http;
using LineWebHookAPI.Configurations;
using LineWebHookAPI.Constants.Auth;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Dto.Yahoo;
using LineWebHookAPI.Models.Job;
using LineWebHookAPI.Models.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
        builder.Services.AddAuthorization();

        var jwtKey = builder.Configuration.GetValue<string>("Auth:JwtKey") ?? string.Empty;
        const int MinimumJwtKeyLength = 32;
        if (jwtKey.Length < MinimumJwtKeyLength)
            throw new InvalidOperationException("Auth:JwtKey must be at least 32 characters.");
        var jwtIssuer = builder.Configuration.GetValue<string>("Auth:Issuer");
        var jwtAudience = builder.Configuration.GetValue<string>("Auth:Audience");
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer
            (
                options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = signingKey,
                        ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
                        ValidAudience = jwtAudience,
                        NameClaimType = ClaimTypes.Name,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            if (context.Request.Cookies.TryGetValue(AuthCookie.Name, out var token))
                                context.Token = token;
                            return Task.CompletedTask;
                        }
                    };
                }
            );
        // 以下DI
        builder.Services.AddTransient<Middleware>();
        builder.Services.AddScoped<LineSignatureFilter>();
        builder.Services.AddScoped<IYahooRepository, YahooRepository>();
        builder.Services.AddScoped<IManagedRepository, ManagedRepository>();
        builder.Services.AddScoped<IMiddleWareRepository, MiddleWareRepository>();
        builder.Services.AddHttpClient<ILineMessagingClient, LineMessagingClient>();
        builder.Services.AddScoped<IAuthService, AuthService>();
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
        app.MapGet
        (
            "/health",
            async (LineWebHookContext dbContext, CancellationToken cancellationToken) =>
            {
                await dbContext.Database.ExecuteSqlRawAsync("SELECT 1", cancellationToken);
                return Results.Ok("ok");
            }
        );

        app.UseMiddleware<Middleware>();

        app.UseHttpsRedirection();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
