using G_Searcher.Configurations;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using G_Searcher.DB;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
// ミドルウェアでDIできるようにする
builder.Services.AddTransient<Middleware>();

// EFCoreの設定
builder.Services.AddDbContext<MyContext>
(
    options => options.UseSqlite("Data Source=G-Searcher.db")
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
