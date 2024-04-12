using DuncanShard.Enums;
using DuncanShard.Models;
using DuncanShard.Models.Buildings;
using DuncanShard.Models.Interfaces;
using Shard.Shared.Core;

namespace DuncanShard.Factories;

public class BuildingFactory
{
    /// <summary>
    /// Factory method for creating a building
    /// </summary>
    /// <param name="type">The building's type</param>
    /// <param name="builder">The builder creating the building</param>
    /// <param name="resourcesCategory"></param>
    /// <returns>A building with the given type</returns>
    /// <exception cref="ArgumentOutOfRangeException">If building's type is wrong</exception>
    /// <exception cref="ArgumentException">If building's type is Mine and there is not resource category specified.</exception>
    public IBuilding CreateBuilding(BuildingType type, IUnit builder, ResourcesCategory? resourcesCategory, IClock clock, User user)
    {
        return type switch
        {
            BuildingType.Mine when resourcesCategory == null => throw new ArgumentException("Mine must have a resource category"),
            BuildingType.Mine => new Mine(builder, resourcesCategory.Value, clock, user),
            BuildingType.Starport => new Starport(builder, clock, user),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}