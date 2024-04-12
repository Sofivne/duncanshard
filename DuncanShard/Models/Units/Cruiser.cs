using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;

namespace DuncanShard.Models.Units;

public class Cruiser : UnitAbstract
{
    private static readonly int ATTACK_TIME = 6, DAMAGE = -10, HP = 400;
    private static readonly UnitType[] PRIORITY = { UnitType.Fighter, UnitType.Cruiser, UnitType.Bomber };
    public Cruiser(System system, Planet? planet, string id, User owner, ICombatOrganiser combatOrganiser) : base(system, planet, id, owner, PRIORITY, DAMAGE, HP, combatOrganiser)
    {
    }

    public override UnitType Type => UnitType.Cruiser;

    public override void Shoot(IUnit target)
    {
        base.Shoot(target);
        base.Shoot(target);
        base.Shoot(target);
        base.Shoot(target);
    }

    protected override bool CanShoot()
    {
        return true;
    }
    
}