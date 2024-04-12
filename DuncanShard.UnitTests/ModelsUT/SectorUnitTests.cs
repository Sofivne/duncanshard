using DuncanShard.Factories;
using DuncanShard.Services;
using Microsoft.Extensions.Options;
using Shard.Shared.Core;

namespace DuncanShard.UnitTests.ModelsUT;

public class SectorUnitTests
{
    /*
     * TODO: Write unit tests for the Sector class
     */
    private IMapBuilderService map = new MapBuilderService(
        Options.Create(new MapGeneratorOptions { Seed = "Test application" }),
        new BuildingFactory());
    
    [Fact]
    public void ShouldReturnCorrectSystem()
    {
        var systems = map.Sector.GetSystems();
        Assert.NotNull(systems);
        var systemsAsList = systems.ToList();
        Assert.NotEmpty(systemsAsList);

        var firstSystem = systemsAsList.First();
        var system = map.Sector.GetSystemByName(firstSystem.Name);
        Assert.NotNull(system);
        Assert.Equal(firstSystem.Name, system.Name);
    }
}