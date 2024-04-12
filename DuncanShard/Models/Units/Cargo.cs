using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Trunks;

namespace DuncanShard.Models.Units;

public class Cargo : UnitAbstract
{

    public Cargo(System system, Planet? planet, string id, User owner) : base(system, planet, id, owner, new DefaultTrunk()) {}

    public Cargo(System system, Planet? planet, string id, User owner, int health, IDictionary<Resource, int> resources)
        : base(system, planet, id, owner, new DefaultTrunk(), health)
    {
        resources.ToList().ForEach(kv => Trunk.AddResource(kv.Key, kv.Value));
    }
    
    public override UnitType Type => UnitType.Cargo;
}