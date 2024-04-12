using System.Collections.Immutable;
using DuncanShard.Models.Interfaces;

namespace DuncanShard.Models.Trunks;

/// <summary>
/// Class representing a trunk. A trunk is a container that can contain resources.
/// </summary>
public abstract class AbstractTrunk : ITrunk
{
    private readonly IDictionary<Resource, int> _resources = new Dictionary<Resource, int>();

    public void AddResource(Resource resource, int quantity)
    {
        if (_resources.ContainsKey(resource))
            _resources[resource] += quantity;
        else
            _resources.Add(resource, quantity);
    }
    
    public void UpdateQuantity(Resource resource, int quantity)
    {
        _resources[resource] = quantity;
    }

    public int RemoveResource(Resource resource, int quantity)
    {
        if (!_resources.ContainsKey(resource))
            return 0;
        var actualQuantity = _resources[resource];
        if (actualQuantity <= quantity)
        {
            _resources.Remove(resource);
            return actualQuantity;
        }
        _resources[resource] -= quantity;
        return quantity;
    }
 
    public int RemoveAllQuantityOfResource(Resource resource)
    {
        if (!_resources.ContainsKey(resource))
            return 0;
        var actualQuantity = _resources[resource];
        _resources.Remove(resource);
        return actualQuantity;
    }

    public ImmutableDictionary<Resource, int> ResourcesQuantity => _resources.ToImmutableDictionary();
    
    public int GetResourceQuantity(Resource resource)
    {
        return !_resources.ContainsKey(resource) ? 0 : _resources[resource];
    }
}