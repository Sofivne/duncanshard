using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;

namespace DuncanShard.Models.Units;

public class Bomber : UnitAbstract
{
    private static readonly int ATTACK_TIME = 60, HP = 50, DAMAGE = -400;
    private static readonly UnitType[] PRIORITY = { UnitType.Cruiser, UnitType.Bomber, UnitType.Fighter };
    public Bomber(System system, Planet? planet, string id, User owner, ICombatOrganiser combatOrganiser) : base(system, planet, id, owner, PRIORITY, DAMAGE, HP, combatOrganiser)
    {
    }

    public override UnitType Type => UnitType.Bomber;
    

    public override void GetShotAt(int damage, IUnit attacker)
    {
        if (attacker.Type == UnitType.Cruiser)
            damage /= 10;
        base.GetShotAt(damage, attacker);
    }

    protected override bool CanShoot()
    {
        return true;
    }
}