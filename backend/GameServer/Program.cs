using GameServer.Application.Common;
using GameServer.Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<EntityStore>();
builder.Services.AddScoped<EntityService>();
builder.Services.AddScoped<ResistanceParser>();
builder.Services.AddScoped<ProficiencyParser>();

var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

if (allowedOrigins is null || allowedOrigins.Length == 0)
{
    allowedOrigins = ["http://localhost:3000"];
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("frontend");
app.MapControllers();
app.Run();