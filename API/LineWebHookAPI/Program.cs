using LineWebHookAPI.Configurations;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using LineWebHookAPI.Models.DB;
using LineWebHookAPI.Models.Http;
using LineWebHookAPI.Models.DB.Repositories;

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
builder.Services.AddScoped<YahooRepositoryBase, YahooRepository>();
builder.Services.AddScoped<HotPepperRepositoryBase, HotPepperRepository>();
builder.Services.AddScoped<ManagedRepositoryBase, ManagedRepository>();
builder.Services.AddScoped<MiddleWareRepositoryBase, MiddleWareRepository>();
builder.Services.AddHttpClient<ILineHttp, LineHttp>();
builder.Services.AddHttpClient<IYahooHttp, YahooHttp>();
builder.Services.AddHttpClient<IHotPepperHttp, HotPepperHttp>();


// Enumを文字列として扱う
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// EFCoreの設定
builder.Services.AddDbContext<LineWebHookContext>
(
    options => options.UseInMemoryDatabase("LineWebHookDB")
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
