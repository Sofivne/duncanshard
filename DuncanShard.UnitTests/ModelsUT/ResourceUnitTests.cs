/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;
using Shard.Shared.Core;

namespace DuncanShard.UnitTests.ModelsUT;

public class ResourceUnitTests
{
    [Fact]
    public void CompareRarity_ShouldReturnCorrectComparison()
    {
        var titaniumResource = new Resource(ResourceKind.Titanium, ResourcesCategory.Solid);
        var goldResource = new Resource(ResourceKind.Gold, ResourcesCategory.Solid);
        var ironResource = new Resource(ResourceKind.Iron, ResourcesCategory.Solid);

        var comparison1 = titaniumResource.CompareRarity(goldResource);
        var comparison2 = goldResource.CompareRarity(ironResource);

        Assert.True(comparison1 < 0); // Titanium is rarer than Gold
        Assert.True(comparison2 < 0); // Gold is rarer than Iron
    }

    [Fact]
    public void FromResourceKind_ShouldCreateCorrectResource()
    {
        const ResourceKind carbonResourceKind = ResourceKind.Carbon;

        var carbonResource = Resource.FromResourceKind(carbonResourceKind);

        Assert.Equal(ResourceKind.Carbon, carbonResource.Kind);
        Assert.Equal(ResourcesCategory.Solid, carbonResource.Category);
    }

    [Fact]
    public void FromResourceKind_ShouldThrowExceptionForUnknownResourceKind()
    {
        const ResourceKind unknownResourceKind = (ResourceKind)100; // Unknown resource kind

        Assert.Throws<ArgumentOutOfRangeException>(() => Resource.FromResourceKind(unknownResourceKind));
    }
}