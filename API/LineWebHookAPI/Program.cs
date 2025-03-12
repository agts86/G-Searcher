using LineWebHookAPI.Configurations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Models.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
// ミドルウェアでDIできるようにする
builder.Services.AddTransient<Middleware>();
// Lineの署名検証用フィルター
builder.Services.AddScoped<LineSignatureFilter>();

// DockerコンテナのときのDB接続文字列
var defaultConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
// appsettings.jsonのDB接続文字列
var configConnection = builder.Configuration.GetConnectionString("DefaultConnection");
// EFCoreの設定
builder.Services.AddDbContext<MyContext>
(
    // 一旦趣味だからSQLiteにしたけど
    // サービス展開考えたらdevとprodで分けるべき
    options => options.UseSqlite(defaultConnection ?? configConnection)
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseMiddleware<Middleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
