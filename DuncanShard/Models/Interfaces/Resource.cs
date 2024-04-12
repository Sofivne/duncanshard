/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using DuncanShard.Enums;
using Shard.Shared.Core;
namespace DuncanShard.Models.Interfaces;

/// <summary>
/// Initializes a new instance of the record.
/// </summary>
/// <param name="Kind">The kind of the resource.</param>
/// <param name="Category">The category of the resource.</param>
public record Resource(ResourceKind Kind, ResourcesCategory Category)
{
    
    /// <summary>
    /// Compares the current resource with another resource based on rarity.
    /// </summary>
    /// <param name="other">The resource to compare with.</param>
    /// <returns>An integer representing the relative order of the resources based on rarity.</returns>
    public int CompareRarity(Resource other)
    {
        var rarityOrder = new List<ResourceKind>
        {
            ResourceKind.Titanium,
            ResourceKind.Gold,
            ResourceKind.Aluminium,
            ResourceKind.Iron,
            ResourceKind.Carbon
        };

        return Comparer<int>.Default.Compare(rarityOrder.IndexOf(Kind), rarityOrder.IndexOf(other.Kind));
    }

    /// <summary>
    /// Creates a new instance based on a specified <see cref="ResourceKind"/>.
    /// </summary>
    /// <param name="resourceKind">The kind of the resource to create.</param>
    /// <returns>A new instance of <see cref="Resource"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the resourceKind is not recognized.</exception>
    public static Resource FromResourceKind(ResourceKind resourceKind)
    {
        return resourceKind switch
        {
            ResourceKind.Carbon => new Resource(ResourceKind.Carbon, ResourcesCategory.Solid),
            ResourceKind.Iron => new Resource(ResourceKind.Iron, ResourcesCategory.Solid),
            ResourceKind.Gold => new Resource(ResourceKind.Gold, ResourcesCategory.Solid),
            ResourceKind.Aluminium => new Resource(ResourceKind.Aluminium, ResourcesCategory.Solid),
            ResourceKind.Titanium => new Resource(ResourceKind.Titanium, ResourcesCategory.Solid),
            ResourceKind.Water => new Resource(ResourceKind.Water, ResourcesCategory.Liquid),
            ResourceKind.Oxygen => new Resource(ResourceKind.Oxygen, ResourcesCategory.Gaseous),
            _ => throw new ArgumentOutOfRangeException(nameof(resourceKind), resourceKind, null)
        };
    }
}