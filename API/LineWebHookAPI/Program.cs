using LineWebHookAPI.Configurations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Models.DB.Repositories;
using LineDevSdk.Configurations;
using LineDevSdk.Https;
using Microsoft.Data.Sqlite;
using LineWebHookAPI.Models.Services;
using LineWebHookAPI.Models.Job;
using LineWebHookAPI.Models.Dto.Yahoo;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
// 以下DI
builder.Services.AddTransient<Middleware>();
builder.Services.AddScoped<LineSignatureFilter>();
builder.Services.AddScoped<IYahooRepository, YahooRepository>();
builder.Services.AddScoped<IManagedRepository, ManagedRepository>();
builder.Services.AddScoped<IMiddleWareRepository, MiddleWareRepository>();
builder.Services.AddHttpClient<ILineHttp, LineHttp>();
builder.Services.AddHttpClient<IYahooHttp, YahooHttp>();
builder.Services.AddScoped<IYahooService, YahooService>();
builder.Services.AddSingleton<IBackgroundJobQueue<YahooLocalJob>, BackgroundJobQueue<YahooLocalJob>>();
builder.Services.AddHostedService<YahooBackgroundService>();


// Enumを文字列として扱う
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// EFCoreの設定
var keepAliveConnection = new SqliteConnection(builder.Configuration.GetConnectionString("SqliteConnection"));
keepAliveConnection.Open();

builder.Services.AddDbContext<LineWebHookContext>
(
    options =>
    options.UseSqlite(keepAliveConnection)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<LineWebHookContext>();
var isMemory = dbContext.Database.GetDbConnection().ConnectionString == "DataSource=:memory:";
if (isMemory)
{
    dbContext.Database.Migrate();
}

app.UseMiddleware<Middleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
