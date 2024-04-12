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

public class NoneTrunkUnitTests
{
    [Fact]
    public void AddResource_ShouldDoNothing()
    {
        var trunk = new NoneTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);

        trunk.AddResource(resource, 10);

        Assert.Equal(0, trunk.GetResourceQuantity(resource));
    }

    [Fact]
    public void RemoveResource_ShouldDoNothing()
    {
        var trunk = new NoneTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);

        var removedQuantity = trunk.RemoveResource(resource, 5);

        Assert.Equal(0, removedQuantity);
    }

    [Fact]
    public void RemoveAllQuantityOfResource_ShouldDoNothing()
    {
        // Arrange
        var trunk = new NoneTrunk();
        var resource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);

        // Act
        var removedQuantity = trunk.RemoveAllQuantityOfResource(resource);

        // Assert
        Assert.Equal(0, removedQuantity);
    }
    
}