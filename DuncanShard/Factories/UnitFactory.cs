using DuncanShard.Enums;
using DuncanShard.Models;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Units;
using Shard.Shared.Core;

namespace DuncanShard.Factories;

public class UnitFactory
{
    /// <summary>
    /// Factory method for creating a unit
    /// </summary>
    /// <param name="system">The system on which the unit currently is</param>
    /// <param name="planet">The planet on which the unit currently is</param>
    /// <param name="type">The unit's type</param>
    /// <param name="user">The owner of the unit</param>
    /// <param name="clock"></param>
    /// <param name="combatOrganiser">The combat organiser we want to add the combat unit into.</param>
    /// <param name="unitId">Null for auto generated id</param>
    /// <param name="health">Unit's health. Only used when unit is Cargo</param>
    /// <param name="resources">The resources to add to the Unit. Only added if unit is Cargo.</param>
    /// <returns>A unit with the given type</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if unit's type is wrong</exception>
    public IUnit CreateUnit(Models.System system, Planet? planet, UnitType type, User user, IClock clock, ICombatOrganiser combatOrganiser, string? unitId = null, int health = 100, IDictionary<Resource, int>? resources = null)
    {
        var id  = unitId ?? Guid.NewGuid().ToString();
        return type switch
        {
            UnitType.Scout => new Scout(system, planet, id, user, clock),
            UnitType.Builder => new Builder(system, planet, id, user, clock),
            UnitType.Bomber => new Bomber(system, planet, id, user, combatOrganiser),
            UnitType.Cruiser => new Cruiser(system, planet, id, user, combatOrganiser),
            UnitType.Fighter => new Fighter(system, planet, id, user, combatOrganiser),
            UnitType.Cargo when resources is not null => new Cargo(system, planet, id, user, health, resources),
            UnitType.Cargo => new Cargo(system, planet, id, user),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}