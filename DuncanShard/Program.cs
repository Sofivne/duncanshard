using DuncanShard.Configuration;
using DuncanShard.Factories;
using DuncanShard.Models;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Units;
using DuncanShard.Services;
using Shard.Shared.Core;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
var config = new DatabaseSettings();
builder.Configuration.Bind(config);


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IMapBuilderService, MapWithWormholesBuilderService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<ICombatOrganiser, CombatOrganiser>();
builder.Services.AddSingleton<UnitFactory>();
builder.Services.AddSingleton<BuildingFactory>();
builder.Services.Configure<MapGeneratorOptions>(builder.Configuration.GetSection(nameof(MapGeneratorOptions)));
builder.Services.Configure<Dictionary<string, Wormhole>>(builder.Configuration.GetSection("Wormholes"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthorization();
app.MapControllers();

app.Run();

namespace DuncanShard
{
    public partial class Program
    {

    }
}
