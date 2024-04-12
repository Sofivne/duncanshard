using DuncanShard.Models.Interfaces;

namespace DuncanShard.Models.Trunks;

/// <summary>
/// Trunk that can't contain anything.
/// </summary>
public class NoneTrunk : AbstractTrunk
{
    public new void AddResource(Resource resource, int quantity)
    {
        // Do nothing
    }
    public new int RemoveResource(Resource resource, int quantity)
    {
        // Do nothing
        return 0;
    }    
    
    public new int RemoveAllQuantityOfResource(Resource resource)
    {
        // Do nothing
        return 0;
    }
}