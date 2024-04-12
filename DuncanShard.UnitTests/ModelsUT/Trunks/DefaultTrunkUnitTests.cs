/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Trunks;
using Shard.Shared.Core;

namespace DuncanShard.UnitTests.ModelsUT.Trunks;

public class DefaultTrunkUnitTests
{
    [Fact]
    public void AddResource_ShouldAddToResources()
    {
        // Arrange
        var trunk = new DefaultTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);

        // Act
        trunk.AddResource(resource, 10);

        // Assert
        Assert.Equal(10, trunk.GetResourceQuantity(resource));
    }
    
    [Fact]
    public void AddResource_ShouldAddToResourcesAfterAlreadyAddingResources()
    {
        var trunk = new DefaultTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);

        trunk.AddResource(resource, 10);
        trunk.AddResource(resource, 10);

        Assert.Equal(20, trunk.GetResourceQuantity(resource));
    }

    [Fact]
    public void UpdateQuantity_ShouldUpdateResourceQuantity()
    {
        var trunk = new DefaultTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);

        trunk.UpdateQuantity(resource, 5);

        Assert.Equal(5, trunk.GetResourceQuantity(resource));
    }

    [Fact]
    public void RemoveResource_ShouldRemoveFromResources()
    {
        var trunk = new DefaultTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);
        trunk.AddResource(resource, 10);

        var removedQuantity = trunk.RemoveResource(resource, 5);

        Assert.Equal(5, removedQuantity);
        Assert.Equal(5, trunk.GetResourceQuantity(resource));
    }

    [Fact]
    public void RemoveAllQuantityOfResource_ShouldRemoveAllFromResources()
    {
        var trunk = new DefaultTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);
        trunk.AddResource(resource, 10);

        var removedQuantity = trunk.RemoveAllQuantityOfResource(resource);

        Assert.Equal(10, removedQuantity);
        Assert.Equal(0, trunk.GetResourceQuantity(resource));
    }

    [Fact]
    public void ResourcesQuantity_ShouldBeImmutable()
    {
        var trunk = new DefaultTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);
        trunk.AddResource(resource, 10);

        var resourcesQuantity = trunk.ResourcesQuantity;

        Assert.Equal(10, resourcesQuantity[resource]);
        
        // Modifying the original dictionary should not affect the actual trunk's state
        trunk.RemoveResource(resource, 5);
        Assert.Equal(10, resourcesQuantity[resource]);
    }

    [Fact]
    public void GetResourceQuantity_ShouldReturnCorrectQuantity()
    {
        var trunk = new DefaultTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);
        trunk.AddResource(resource, 10);

        var quantity = trunk.GetResourceQuantity(resource);

        Assert.Equal(10, quantity);
    }
    
    [Fact]
    public void RemoveAllQuantityOfResource_RemoveNotPresentResourceReturnsZeroQuantityRemoved()
    {
        var trunk = new DefaultTrunk();
        var resource = Resource.FromResourceKind(ResourceKind.Iron);
        
        var removedQuantity = trunk.RemoveAllQuantityOfResource(resource);

        Assert.Equal(0, removedQuantity);
    }
    
    [Fact]
    public void RemoveResource_RemoveNotPresentResourceReturnsZeroQuantityRemoved()
    {
        var trunk = new DefaultTrunk();
        var resource = Resource.FromResourceKind(ResourceKind.Iron);

        var removedQuantity = trunk.RemoveResource(resource, 5);

        Assert.Equal(0, removedQuantity);
    }
    
    [Fact]
    public void RemoveResource_RemoveMoreQuantityThanPresent()
    {
        // Arrange
        var trunk = new DefaultTrunk();
        var resource = Resource.FromResourceKind(ResourceKind.Iron);
        trunk.AddResource(resource, 10);
        
        // Act
        var removedQuantity = trunk.RemoveResource(resource, 15);

        // Assert
        Assert.Equal(10, removedQuantity);
    }

}