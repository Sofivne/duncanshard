/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;
using Shard.Shared.Core;

namespace DuncanShard.Models.Units;

public class CombatOrganiser : ICombatOrganiser
{
    private IClock Clock { get; }
    private List<IUnit> Units { get; } = new();

    private static readonly int ATTACK_TIME = 6;
    /// <summary>
    /// Adds an attacker to the list of units that will fight.
    /// </summary>
    /// <param name="unit">The unit we want to add</param>
    public void AddAttacker(IUnit unit)
    {
        Units.Add(unit);
    }
    
    /// <summary>
    /// Constructor of the combat organiser. It will find and organise the fights between units.
    /// </summary>
    /// <param name="clock">The clock used to create a timer</param>
    public CombatOrganiser(IClock clock)
    {
        Clock = clock;
        Clock.CreateTimer(
            callback: _ => Organise(),
            state: null,
            dueTime: TimeSpan.FromSeconds(ATTACK_TIME - Clock.Now.Second % ATTACK_TIME),
            period: TimeSpan.FromSeconds(ATTACK_TIME)
        );
    }
    
    /// <summary>
    /// Organise the fights between units
    /// </summary>
    private void Organise()
    {
        var unitsFightingNow = new List<IUnit>(Units);
        if (Clock.Now.Second != 0) // If we are not at the beginning of the minute, we don't want bomber to fight
            unitsFightingNow.RemoveAll(u => u.Type == UnitType.Bomber);
        unitsFightingNow.Sort((x, y) => x.Damage - y.Damage);
        
        var fights = new Dictionary<IUnit, IUnit>();
        // TODO : Refactor with linq
        foreach (var attacker in unitsFightingNow)
        {
            var target = GetTarget(attacker);
            if (target == null)
                continue;
            fights.Add(attacker, target);
        }
        foreach (var (attacker, target) in fights)
            attacker.Shoot(target);
        // Console.WriteLine(Units.Where(u => u.Health<=0).Select(u => u.Health).ToList());

        Units.RemoveAll(u => u.Health <= 0);
    }

    /// <summary>
    /// Gets the target of the attacker.
    /// </summary>
    /// <param name="attacker">The attacker</param>
    /// <returns>The best target. If there is no target, returns null.</returns>
    private IUnit? GetTarget(IUnit attacker)
    {
        var targets = GetOtherUnits().ToList();
        var priority = attacker.Priority.ToList();
        targets.Sort((x, y) => priority.IndexOf(x.Type) - priority.IndexOf(y.Type));
        var target = targets.FirstOrDefault();
        return target;

        IEnumerable<IUnit> GetOtherUnits()
        {
            return Units
                .Where(u => u.Planet != null
                    ? u.Planet?.Name == attacker.Planet?.Name
                    : u.System.Name == attacker.System.Name)
                .Where(u => u.Id != attacker.Id && u.Owner.Id != attacker.Owner.Id);
        }
    }
}