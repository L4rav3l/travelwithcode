using DotNetEnv;
using TravelWithCode.Infrastructure;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<Postgresql>();
builder.Services.AddSingleton<Argon2>();
builder.Services.AddSingleton<JsonWebToken>();
builder.Services.AddSingleton<Ciper>();

builder.Services.AddScoped<AuthorizationFilter>();

var app = builder.Build();

app.MapControllers();
app.Run();