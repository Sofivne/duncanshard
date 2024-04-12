using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;
using Shard.Shared.Core;

namespace DuncanShard.Models.Buildings;

public abstract class BuildingAbstract : IBuilding
{
    /// <summary>
    /// Constructor for a building
    /// </summary>
    /// <param name="builder">The unit creating the building</param>
    /// <param name="resourcesCategory">The resource type. Only used for mine.</param>
    /// <param name="clock">Clock to create building task.</param>
    /// <exception cref="ArgumentException">If builder isn't on a planet</exception>
    protected BuildingAbstract(IUnit builder, ResourcesCategory? resourcesCategory, IClock clock)
    {
        CancellationTokenSource = new CancellationTokenSource();
        if (builder.Planet == null)
            throw new ArgumentException("Builder is not on a planet");
        Id = new Guid().ToString();
        Builder = builder;
        Planet = builder.Planet;
        System = builder.System;
        IsBuilt = false;
        Clock = clock;
        ResourcesCategory = resourcesCategory;
        
        EstimatedBuildTime = clock.Now.AddMinutes(5);
    }
    
    private CancellationTokenSource CancellationTokenSource { get; }
    
    protected abstract Task BuildingTask { get; init; }
    public Task AwaitBuilding()
    {
        return BuildingTask;  
    }     
    public string Id { get; }
    public IUnit Builder { get; }
    
    protected IClock Clock { get; }
    
    public ResourcesCategory? ResourcesCategory { get; }
    public Planet Planet { get; }
    public System System { get; }
    public abstract string Type { get; }
    private static readonly int BUILDING_TIME_MS = 60000 * 5;
    
    public DateTime? EstimatedBuildTime { get; protected set; }
    public bool IsBuilt { get; protected set; }
    
    public void CancelBuilding()
    {
        EstimatedBuildTime = null;
        CancellationTokenSource.Cancel();
    }

    public abstract IUnit? Use(UnitType buildingType);

    protected async Task Build()
    {
        await Clock.Delay(BUILDING_TIME_MS, CancellationTokenSource.Token);
        IsBuilt = true;
        EstimatedBuildTime = null;
    }
    
}