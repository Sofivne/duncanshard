using DuncanShard.Enums;
using Shard.Shared.Core;

namespace DuncanShard.Models.Units;

public class Builder : UnitAbstract
{
    public Builder(System system, Planet? planet, string id, User owner, IClock clock) : base(system, planet, id, owner) {}
    public override UnitType Type => UnitType.Builder;
    
}