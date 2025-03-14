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

// EFCoreの設定
builder.Services.AddDbContext<MyContext>
(
    // 一旦趣味だからSQLiteにしたけど
    // サービス展開考えたらdevとprodで分けるべき
    options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "v1"));
}
app.Use(async (context, next) =>
    {
        context.Request.EnableBuffering(); // リクエストボディを再読込可能に
        context.Request.Body.Position = 0;
        await next(); 
    });
app.UseMiddleware<Middleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MyContext>();
    dbContext.Database.Migrate(); // DBを作成・更新する
}

app.Run();
