using DotNetEnv;
using TravelWithCode.Infrastructure;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<Postgresql>();
builder.Services.AddSingleton<Argon2>();
builder.Services.AddSingleton<JsonWebToken>();
builder.Services.AddSingleton<Ciper>();
builder.Services.AddSingleton<SSHService>();
builder.Services.AddSingleton<ILxcTaskQueue, LxcTaskQueue>();

builder.Services.AddScoped<AuthorizationFilter>();

builder.Services.AddHostedService<GarbageCollector>();
builder.Services.AddHostedService<LxcSetupWorker>();

builder.Services.AddHttpClient<ProxmoxService>(client =>
{
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});

var app = builder.Build();

app.MapControllers();
app.Run();