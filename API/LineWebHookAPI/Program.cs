using LineWebHookAPI.Configurations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Models.Services.Yahoo;
using LineWebHookAPI.Models.DB.Repositories;
using LineWebHookAPI.Models.Services.HotPeppers;
using LineWebHookAPI.Models.Services.Managed;

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
builder.Services.AddScoped<IHttpAdapter,HttpAdapter>();
builder.Services.AddScoped<IYahooService, YahooService>();
builder.Services.AddScoped<IHotPepperService, HotPepperService>();
builder.Services.AddScoped<IManagedService, ManagedService>();
builder.Services.AddScoped<YahooRepositoryBase, YahooRepository>();
builder.Services.AddScoped<HotPepperRepositoryBase, HotPepperRepository>();
builder.Services.AddScoped<ManagedRepositoryBase, ManagedRepository>();
builder.Services.AddScoped<BaseControllerRepository, BaseControllerRepository>();
builder.Services.AddScoped<MiddleWareRepositoryBase, MiddleWareRepository>();

// Enumを文字列として扱う
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// EFCoreの設定
builder.Services.AddDbContext<LineWebHookContext>
(
    // 一旦趣味だからSQLiteにしたけど
    // サービス展開考えたらdevとprodで分けるべき
    options => options.UseSqlite(builder.Configuration.GetConnectionString("SqliteConnection"))
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

// DBを作成・更新する
using var scope = app.Services.CreateScope();

var dbContext = scope.ServiceProvider.GetRequiredService<LineWebHookContext>();
dbContext.Database.Migrate(); 


app.Run();
