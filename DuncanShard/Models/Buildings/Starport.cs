using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;
using Shard.Shared.Core;

namespace DuncanShard.Models.Buildings;

/// <summary>
/// Building that creates units.
/// </summary>
public class Starport : BuildingAbstract
{
    private readonly User _user;
    
    /// <summary>
    /// Constructor of the "starport".
    /// </summary>
    /// <param name="builder">The unit building this building.</param>
    /// <param name="clock">Clock to create building task.</param>
    /// <param name="user">The owner of this building.</param>
    /// <exception cref="ArgumentException">If builder isn't on a planet</exception>
    public Starport(IUnit builder, IClock clock, User user) : base(builder, null, clock)
    {
        _user = user;
        BuildingTask = base.Build();
    }

    protected sealed override Task BuildingTask { get; init; }

    public override string Type => "starport";
    public override IUnit Use(UnitType unitType)
    {
        if (!IsBuilt)
            throw new InvalidOperationException("Building is not built yet");
        if (!_costs.ContainsKey(unitType))
            throw new InvalidOperationException("Unit type is not valid"); 
        var cost = _costs[unitType];
        foreach (var (resource, quantity) in cost)
            if (_user.GetQuantityOfResource(resource) < quantity)
                throw new InvalidOperationException("Not enough resources");

        foreach (var (resource, quantity) in cost)
            _user.RemoveResource(resource, quantity);
        
        return _user.AddUnit(System, Planet, unitType);

    }
    
    // TODO : faire un JSON conf
    private readonly IDictionary<UnitType, IList<KeyValuePair<Resource, int>>> _costs = new Dictionary<UnitType, IList<KeyValuePair<Resource, int>>>
    {
        {
            UnitType.Builder, new List<KeyValuePair<Resource, int>>
            {
                new(Resource.FromResourceKind(ResourceKind.Carbon), 5),
                new(Resource.FromResourceKind(ResourceKind.Iron), 10),
            }
        },
        {
            UnitType.Fighter, new List<KeyValuePair<Resource, int>>
            {
                new(Resource.FromResourceKind(ResourceKind.Aluminium), 5),
                new(Resource.FromResourceKind(ResourceKind.Iron), 20),
            }
        },
        {
            UnitType.Bomber, new List<KeyValuePair<Resource, int>>
            {
                new(Resource.FromResourceKind(ResourceKind.Titanium), 10),
                new(Resource.FromResourceKind(ResourceKind.Iron), 30),
            }
        },
        {
            UnitType.Cruiser, new List<KeyValuePair<Resource, int>>
            {
                new(Resource.FromResourceKind(ResourceKind.Iron), 60),
                new(Resource.FromResourceKind(ResourceKind.Gold), 20),
            }
        },
        {
            UnitType.Scout, new List<KeyValuePair<Resource, int>>
            {
                new(Resource.FromResourceKind(ResourceKind.Carbon), 5),
                new(Resource.FromResourceKind(ResourceKind.Iron), 5),
            }
        },
        {
            UnitType.Cargo, new List<KeyValuePair<Resource, int>>
            {
                new(Resource.FromResourceKind(ResourceKind.Carbon), 10),
                new(Resource.FromResourceKind(ResourceKind.Iron), 10),
                new(Resource.FromResourceKind(ResourceKind.Gold), 5),
            }
        },
    };

}