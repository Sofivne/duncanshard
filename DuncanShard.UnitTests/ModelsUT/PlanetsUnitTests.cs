using DuncanShard.Factories;
using DuncanShard.Models;
namespace DuncanShard.UnitTests.ModelsUT;
using Shard.Shared.Core;


public class PlanetTests
{
    /*
     * Planet cannot be initialized with null PlanetSpecification
     */
    [Fact]
    public void Planet_ObjectCannotBeInitialized_WithNullPlanetSpecification()
    {
        PlanetSpecification? planetSpecification = null;

        Assert.Throws<ArgumentNullException>(() => new Planet(planetSpecification, new BuildingFactory()));
    }
    
    // TODO : Add more tests for the Planet class
}

