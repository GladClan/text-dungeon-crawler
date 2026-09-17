using GameServer.Application.Services;
using GameServer.Domain.Entities;
using GameServer.Domain.Entities.BeastiaryEntity;
using GameServer.Domain.Entities.BeastiaryEntity.BestiaryLibrary;
using GameServer.Domain.Entities.EntityAI.AILibrary;
using GameServer.Domain.Items;
using GameServer.Domain.Items.ItemsLibrary;
using GameServer.Domain.Map;
using GameServer.Domain.Map.Scene.InitialReleaseScenes;
using GameServer.Domain.Skills;
using GameServer.Domain.Skills.SkillsLibrary;
using GameServer.Domain.Statistics;
using GameServer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<EntityStore>();
builder.Services.AddSingleton<StatisticsTracker>();
builder.Services.AddSingleton(new GameContext(
    scenes: [new QuickTestSceneObject(partyId: "player-party")],
    gameContextState: new TestGameContextState()
));

builder.Services.AddSingleton<IBestiaryIndex, BestiaryIndex>();
builder.Services.AddSingleton<ISkillsIndex, SkillsIndex>();
builder.Services.AddSingleton<IItemsIndex, ItemsIndex>();
builder.Services.AddSingleton<IEntityAIIndex, EntitAIIndex>();

builder.Services.AddScoped<EntityService>();
builder.Services.AddScoped<StatisticsService>();
builder.Services.AddScoped<EntityStatsService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<SkillService>();
builder.Services.AddScoped<CombatService>();
builder.Services.AddScoped<BattleService>();
builder.Services.AddScoped<EventServices>();

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