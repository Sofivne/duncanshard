using DuncanShard.Enums;

namespace DuncanShard.Models.Interfaces;

public interface IBuilding
{
    public string Id { get; }
    public IUnit Builder { get; }
    public  ResourcesCategory? ResourcesCategory { get; }
    public Planet Planet { get; }
    public System System { get; }
    public string Type { get; }
    
    public DateTime? EstimatedBuildTime { get; }
    public bool IsBuilt { get; }
    /// <summary>
    /// Cancels the building of the building.
    /// </summary>
    public void CancelBuilding();
    /// <summary>
    ///  Uses the building.
    /// </summary>
    /// <param name="unitType">Used by some implementation to determine what to build.</param>
    /// <returns>Null if the building doesn't build Unit. The brand new Unit otherwise.</returns>
    public IUnit? Use(UnitType unitType);
    /// <summary>
    /// Returns a task that completes when the building is built.
    /// </summary>
    /// <returns>A task that completes when the building is built.</returns>
    public Task AwaitBuilding();

}