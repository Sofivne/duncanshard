using System.Text.Json.Serialization;
using DuncanShard.Enums;
using Shard.Shared.Core;

namespace DuncanShard.Models.Interfaces;

public interface IUnit
{
    /// <summary>
    /// Awaits the unit's arrival
    /// </summary>
    public Task AwaitArrival();
    /// <summary>
    /// Moves the unit to the destination system and planet if there's one given
    /// </summary>
    /// <param name="destinationSystem">The destination system</param>
    /// <param name="destinationPlanet">The destination planet.</param>
    /// <param name="clock">The clock needed to await the travel time</param>
    public Task MoveUnit(System destinationSystem, Planet? destinationPlanet, IClock clock);
    /// <summary>
    /// Method to know if the unit can build a building (it's on a planet and it's a builder)
    /// </summary>
    /// <returns>True if unit can build, false otherwise</returns>
    public bool CanBuild();
    /// <summary>
    /// Used to get shot at by another unit. Can either add health point or remove health points.
    /// </summary>
    /// <param name="damage">The number of health points to remove or add. Negative value to remove.</param>
    /// <param name="attacker">The unit attacking.</param>
    public void GetShotAt(int damage, IUnit attacker);
    /// <summary>
    /// Shoot at another unit.
    /// </summary>
    /// <param name="target">The target we want to hit.</param>
    public void Shoot(IUnit target);
    /// <summary>
    /// Used by combat units. A list of units to hit first. First unit in the list is the first to be hit. Last unit in the list is the last to be hit.
    /// </summary>
    public UnitType[] Priority { get; }
    /// <summary>
    /// Adds resource to trunk.
    /// </summary>
    /// <param name="resource">The resource to add.</param>
    /// <param name="quantity">The quantity to add.</param>
    public void AddToTrunk(Resource resource, int quantity);
    /// <summary>
    /// Removes resource from trunk.
    /// </summary>
    /// <param name="resource">The resource to remove.</param>
    /// <param name="quantity">The quantity to remove.</param>
    /// <returns>The actual removed quantity</returns>
    public int RemoveFromTrunk(Resource resource, int quantity);
    /// <summary>
    /// Gets the quantity of a resource in the trunk.
    /// </summary>
    /// <param name="resource">The resource to retrieves from the trunk.</param>
    /// <returns>The quantity of the given resource in the trunk.</returns>
    public int GetResourceQuantityInTrunk(Resource resource);

    public void UpdateResourceInTrunk(Resource resource, int quantity);
    /// <summary>
    ///  The unit's damage per hit. Negative value to remove health points.
    /// </summary>
    public int Damage { get; }
    public System System { get; }
    public User Owner { get; }
    public DateTime? EstimatedTimeOfArrival { get; }
    public System? DestinationSystem { get; } // A unit has to be in a system
    public Planet? DestinationPlanet { get; } // A unit can be in a system without being on a planet
    public string? DestinationShard { get; } // A unit can be in a system without being on a planet
    public string Id { get; }
    public Planet? Planet { get; }
    public int Health { get; }
    public ITrunk Trunk { get; }

    
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public UnitType Type { get; }

}