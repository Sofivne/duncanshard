using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;
using Shard.Shared.Core;

namespace DuncanShard.Models.Buildings;

public class Mine : BuildingAbstract
{
    private static readonly int MINING_TIME = 60;
    private readonly User _user;
    
    /// <summary>
    /// Constructor of the mine.
    /// </summary>
    /// <param name="builder">The unit building this building.</param>
    /// <param name="resourcesCategory">The resource type that can be mined with this mine.</param>
    /// <param name="clock">Clock to create building task.</param>
    /// <param name="user">The owner of this building.</param>
    /// <exception cref="ArgumentException">If builder isn't on a planet</exception>
    public Mine(IUnit builder, ResourcesCategory resourcesCategory, IClock clock, User user) : base(builder, resourcesCategory, clock)
    {
        _user = user;
        BuildingTask = _build();
    }

    /// <summary>
    /// Builds the mine and start mining.
    /// </summary>
    private async Task _build()
    {
        await base.Build();
        Clock.CreateTimer(
            callback: _ => Use(UnitType.Scout),
            state: null,
            dueTime: TimeSpan.FromSeconds(MINING_TIME),
            period:TimeSpan.FromSeconds(MINING_TIME)
        );
    }

    protected sealed override Task BuildingTask { get; init; }
    public override string Type => "mine";

    public override IUnit? Use(UnitType unitType)
    {
        Resource resourceKind;
        try
        {
            resourceKind = Planet.RemoveOneUnitOfMostPresentResource(ResourcesCategory!.Value);
        }
        catch (InvalidOperationException)
        {
            return null;
        }
        _user.AddResource(resourceKind);
        return null;

    }
}