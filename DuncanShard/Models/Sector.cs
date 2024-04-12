using DuncanShard.Factories;
using Shard.Shared.Core;

namespace DuncanShard.Models;

public class Sector
{
    private IEnumerable<System> Systems { get; }
    
    /// <summary>
    /// Returns all the systems in the sector.
    /// </summary>
    /// <returns>All sector's systems.</returns>
    public IEnumerable<System> GetSystems() => Systems;
    
    /// <summary>
    /// Returns a system corresponding to the name given if found.
    /// </summary>
    /// <param name="systemName">The system to find.</param>
    /// <returns>The system if found, null otherwise.</returns>
    public System? GetSystemByName(string systemName) => Systems.FirstOrDefault(system => system.Name == systemName);

    /// <summary>
    /// The constructor for the Sector class
    /// </summary>
    /// <param name="sectorSpecification">The Sector type from the core project.</param>
    /// <param name="buildingFactory">The factory used to create buildings.</param>
    public Sector(SectorSpecification sectorSpecification, BuildingFactory buildingFactory)
    {
        Systems = sectorSpecification.Systems.Select(systemSpecification => new System(systemSpecification, buildingFactory)).ToList();
        // SectorSpecification = sectorSpecification;
    }
    
    

    /// <summary>
    /// Gets a random "system" from the sector
    /// </summary>
    /// <returns>The random system</returns>
    public System GetRandomSystem()
    {
        var random = new Random();
        var systems = Systems.ToList();
        var chosenSystemIndex = random.Next(systems.Count);
        return systems[chosenSystemIndex];
    }
}