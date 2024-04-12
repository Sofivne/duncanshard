/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using System.Collections.Immutable;
using DuncanShard.Enums;
using DuncanShard.Models.DTOs;
using DuncanShard.Models.Interfaces;
using Shard.Shared.Core;

namespace DuncanShard.Models.Mappers;

public static class MapperMethodsExtension
{
    public static BuildingDto ToBuildingDto(this IBuilding building)
        => new(building.Id, building.IsBuilt, building.Type, building.Planet.Name,
            building.System.Name, building.EstimatedBuildTime, building.ResourcesCategory.ToString()?.ToLower()
        );

    public static UnitDto ToUnitDto(this IUnit unit)
        => new(unit.Id, unit.Type.ToString().ToLower(), unit.Planet?.Name, unit.System.Name,
            unit.DestinationPlanet?.Name, unit.DestinationSystem?.Name, unit.Health,
            unit.DestinationShard,
            unit.Trunk.ResourcesQuantity.ToImmutableDictionary(k => k.Key.Kind.ToString().ToLower(), v => v.Value)
        );
    
    public static UserDto ToUserDto(this User user)
        => new(user.Id, user.Pseudo, user.DateOfCreation,
            user.SafeResourcesQuantity
                .Select(kv => new KeyValuePair<ResourceKind, int>(kv.Key.Kind, kv.Value))
                .ToDictionary(key => key.Key.ToString().ToLower(), v => v.Value)
        );
    
    public static SystemDto ToSystemDto(this System system) => new(system.Name, system.GetPlanets());
    
    public static UnitLocationDto ToUnitLocationDto(this IUnit unit) 
        => new(unit.System.Name, 
            unit.Planet?.Name,
            unit.Type != UnitType.Builder ? 
                (unit.Planet?.ResourcesQuantity ?? new Dictionary<Resource, int>().ToImmutableDictionary())
                    .ToImmutableDictionary(k => k.Key.Kind.ToString().ToLower(), v => v.Value) 
                : null
        );
}