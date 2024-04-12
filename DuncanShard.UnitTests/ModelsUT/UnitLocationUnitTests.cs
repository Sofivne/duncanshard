using DuncanShard.Enums;
using DuncanShard.Factories;
using DuncanShard.Models;
using DuncanShard.Models.Mappers;
using DuncanShard.Models.Units;
using DuncanShard.Services;
using Microsoft.Extensions.Options;
using Shard.Shared.Core;

namespace DuncanShard.UnitTests.ModelsUT;

public class UnitLocationUnitTests
{
    
    /*
     * Creating a new UnitLocation object with valid parameters should set the System, Planet, and ResourcesQuantity properties correctly. 
     */
    [Fact]
    public void ShouldSetPropertiesCorrectly()
    {
        var options = new MapGeneratorOptions
        {
            Seed = "Test application"
        };
        var map = new MapBuilderService(Options.Create(options), new BuildingFactory());
        Assert.NotNull(map.Wormholes);
        var clock = new SystemClock();
        var user = new User("id", "pseudo", map, clock, new CombatOrganiser(clock), null, new UnitFactory());
        var builder = user.UserUnits.FirstOrDefault(u => u.Type == UnitType.Builder);
        var scout = user.UserUnits.FirstOrDefault(u => u.Type == UnitType.Scout);
        Assert.NotNull(builder);
        Assert.NotNull(scout);

        Assert.Null(builder.ToUnitLocationDto().ResourcesQuantity);
        Assert.NotNull(scout.ToUnitLocationDto().ResourcesQuantity);
    }
    
}