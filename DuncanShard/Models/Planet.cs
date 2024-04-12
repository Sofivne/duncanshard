using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DuncanShard.Enums;
using DuncanShard.Factories;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.RequestBodies;
using Shard.Shared.Core;

namespace DuncanShard.Models;

public class Planet
{
    private readonly BuildingFactory _buildingFactory;
    private readonly Dictionary<Resource, int> _resourcesQuantity;
    public string Name { get; private init; }
    public int Size { get; private init; }

    [JsonIgnore]
    public ImmutableDictionary<Resource, int> ResourcesQuantity => _resourcesQuantity.ToImmutableDictionary();

    private IList<IBuilding> Buildings { get; }
    
    public IEnumerable<IBuilding> GetBuildings() => Buildings;
    
    /// <summary>
    /// Removes a building from the planet.
    /// </summary>
    /// <param name="building">The building to remove.</param>
    public void DestroyBuilding(IBuilding building) => Buildings.Remove(building);
    
    /// <summary>
    /// The constructor for the <see cref="Planet"/>
    /// </summary>
    /// <param name="planetSpecification">The Planet type from the core project</param>
    public Planet(PlanetSpecification planetSpecification, BuildingFactory buildingFactory)
    {
        if (planetSpecification == null)
            throw new ArgumentNullException(nameof(planetSpecification), "Planet specification cannot be null.");
        Name = planetSpecification.Name;
        Size = planetSpecification.Size;

        _resourcesQuantity = planetSpecification.ResourceQuantity
            .Select(dict =>
                new KeyValuePair<Resource, int>(
                    Resource.FromResourceKind(dict.Key),
                    dict.Value))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        
        Buildings = new List<IBuilding>();
        _buildingFactory = buildingFactory;
    }

    
    /// <summary>
    /// Creates a building on the planet.
    /// </summary>
    /// <param name="buildingType">The type of the building to be created.</param>
    /// <param name="builder">The unit that will build the building.</param>
    /// <param name="resourceCategory">The resource type that the building can use.</param>
    /// <param name="clock">The clock used for time-related operations.</param>
    /// <param name="user">The user who initiates the building creation.</param>
    /// <returns>The created building as an IBuilding instance.</returns>
    public IBuilding CreateBuilding(BuildingType buildingType, IUnit builder, ResourcesCategory? resourceCategory,
        IClock clock, User user)
    {
        var building = _buildingFactory.CreateBuilding(buildingType, builder,
            resourceCategory, clock, user);
        Buildings.Add(building);
        return building;
    }

    /// <summary>
    /// gets the most present resource of the given category.
    /// </summary>
    /// <param name="resourcesCategory"> The category of the resource to get.</param>
    /// <returns>Null if most present resource quantity left is 0. Otherwise, the resource kind removed.</returns>
    private Resource _getBestResource(ResourcesCategory resourcesCategory)
    {
        
        var mostPresentResourceOfCategory = ResourcesQuantity
            .Where(kv => kv.Key.Category == resourcesCategory)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
        
       var mostPresentResource = mostPresentResourceOfCategory
            .Where(kv => kv.Value == mostPresentResourceOfCategory.Values.Max())
            .Aggregate((x, y) => x.Key.CompareRarity(y.Key) < 0  ? x : y);

        return mostPresentResource.Key;
    }

    /// <summary>
    /// Removes one unit of the most present resource of the given category.
    /// </summary>
    /// <param name="resourcesCategory">The category of the resource to be removed.</param>
    /// <returns>The resource kind that was removed.</returns>
    /// <exception cref="InvalidOperationException">If there is no more resource to remove.</exception>
    public Resource RemoveOneUnitOfMostPresentResource(ResourcesCategory resourcesCategory)
    {
        var resourceToRemove = _getBestResource(resourcesCategory);
        _resourcesQuantity.TryGetValue(resourceToRemove, out var resourceQty);
        if (resourceQty <= 0)
            throw new InvalidOperationException("No more resource to remove.");
        _resourcesQuantity[resourceToRemove] -= 1;
        return resourceToRemove;
    }
}