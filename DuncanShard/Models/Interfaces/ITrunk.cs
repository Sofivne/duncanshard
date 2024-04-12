using System.Collections.Immutable;

namespace DuncanShard.Models.Interfaces;

/// <summary>
/// Interface representing a trunk. A trunk is a container for resources.
/// </summary>
public interface ITrunk
{
    /// <summary>
    /// Adds resource to the trunk.
    /// </summary>
    /// <param name="resource">The resource to add.</param>
    /// <param name="quantity">The resource's quantity to add.</param>
    public void AddResource(Resource resource, int quantity);
    /// <summary>
    /// Update the quantity of a resource in the trunk.
    /// </summary>
    /// <param name="resource">The resource to update.</param>
    /// <param name="quantity">The new quantity to put.</param>
    public void UpdateQuantity(Resource resource, int quantity);

    /// <summary>
    /// Removes resource from the trunk.
    /// </summary>
    /// <param name="resource">The resource to remove.</param>
    /// <param name="quantity">The resource's quantity to remove.</param>
    /// <returns>The actual quantity removed.</returns>
    public int RemoveResource(Resource resource, int quantity);
    
    /// <summary>
    /// Removes all quantity of a resource from the trunk.
    /// </summary>
    /// <param name="resource">The resource to remove.</param>
    /// <returns>The quantity removed.</returns>
    public int RemoveAllQuantityOfResource(Resource resource);
    
    public ImmutableDictionary<Resource, int> ResourcesQuantity { get; }
    
    /// <summary>
    /// Get the quantity of a resource in the trunk.
    /// </summary>
    /// <param name="resource">The resource to find.</param>
    /// <returns>The quantity of the resource.</returns>
    public int GetResourceQuantity(Resource resource);

}