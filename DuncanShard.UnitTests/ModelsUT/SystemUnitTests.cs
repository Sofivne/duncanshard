
using DuncanShard.Factories;
using DuncanShard.Models.Interfaces;
using Moq;
using Shard.Shared.Core;

namespace DuncanShard.UnitTests.ModelsUT;

public class SystemUnitTests
{
    [Fact]
    public void GetPlanets_ShouldReturnAllPlanets()
    {
        // Arrange
        var planetSpec1 = new Mock<PlanetSpecification>();
        planetSpec1.SetupGet(p => p.Name).Returns("Planet1");

        var planetSpec2 = new Mock<PlanetSpecification>();
        planetSpec2.SetupGet(p => p.Name).Returns("Planet2");

        var planetSpec3 = new Mock<PlanetSpecification>();
        planetSpec3.SetupGet(p => p.Name).Returns("Planet3");

        var systemSpecificationMock = new Mock<SystemSpecification>();
        systemSpecificationMock.SetupGet(s => s.Name).Returns("TestSystem");
        systemSpecificationMock.SetupGet(s => s.Planets).Returns(new List<PlanetSpecification>
        {
            planetSpec1.Object,
            planetSpec2.Object,
            planetSpec3.Object
        });

        var buildingFactory = new BuildingFactory();

        var system = new Models.System(systemSpecificationMock.Object, buildingFactory);

        // Act
        var planets = system.GetPlanets();

        // Assert
        Assert.Equal(3, planets.Count());
    }

    [Fact]
    public void GetPlanetByName_ShouldReturnCorrectPlanet()
    {
        // Arrange
        var planetSpec1 = new Mock<PlanetSpecification>();
        planetSpec1.SetupGet(p => p.Name).Returns("Planet1");

        var planetSpec2 = new Mock<PlanetSpecification>();
        planetSpec2.SetupGet(p => p.Name).Returns("Planet2");

        var planetSpec3 = new Mock<PlanetSpecification>();
        planetSpec3.SetupGet(p => p.Name).Returns("Planet3");

        var systemSpecificationMock = new Mock<SystemSpecification>();
        systemSpecificationMock.SetupGet(s => s.Name).Returns("TestSystem");
        systemSpecificationMock.SetupGet(s => s.Planets).Returns(new List<PlanetSpecification>
        {
            planetSpec1.Object,
            planetSpec2.Object,
            planetSpec3.Object
        });

        var buildingFactory = new BuildingFactory();

        var system = new Models.System(systemSpecificationMock.Object, buildingFactory);

        // Act
        var planet = system.GetPlanetByName("Planet2");

        // Assert
        Assert.NotNull(planet);
        Assert.Equal("Planet2", planet!.Name);
    }
    
    [Fact]
    public void WelcomeUnit_ShouldAddUnitToSystem()
    {
        // Arrange
        var systemSpecificationMock = new Mock<SystemSpecification>();
        var buildingFactory = new BuildingFactory();

        var system = new Models.System(systemSpecificationMock.Object, buildingFactory);
        var unitMock = new Mock<IUnit>();

        // Act
        system.WelcomeUnit(unitMock.Object);

        // Assert
        Assert.Contains(unitMock.Object, system.GetUnits());
    }

    [Fact]
    public void RemoveUnit_ShouldRemoveUnitFromSystem()
    {
        // Arrange
        var systemSpecificationMock = new Mock<SystemSpecification>();
        var buildingFactory = new BuildingFactory();

        var system = new Models.System(systemSpecificationMock.Object, buildingFactory);
        var unitMock = new Mock<IUnit>();

        // Act
        system.WelcomeUnit(unitMock.Object);
        Assert.Contains(unitMock.Object, system.GetUnits());
        system.RemoveUnit(unitMock.Object);

        // Assert
        Assert.DoesNotContain(unitMock.Object, system.GetUnits());
    }
}