using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;

namespace DuncanShard.Models.Units;

/// <summary>
/// A fighter (aka "Chasseur") is a unit that can shoot and has 80 hp.
/// </summary>
public class Fighter : UnitAbstract
{
    private static readonly int ATTACK_TIME = 6, HP = 80, DAMAGE = -10;
    private static readonly UnitType[] PRIORITY = { UnitType.Bomber, UnitType.Fighter, UnitType.Cruiser };

    public Fighter(System system, Planet? planet, string id, User owner, ICombatOrganiser combatOrganiser) : base(system, planet, id, owner, PRIORITY, DAMAGE, HP, combatOrganiser)
    {
    }

    public override UnitType Type => UnitType.Fighter;

    protected override bool CanShoot()
    {
        return true;
    }
}