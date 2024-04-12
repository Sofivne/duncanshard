/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using DuncanShard.Models.RequestBodies;

namespace DuncanShard.UnitTests.ModelsUT;

public class CreateBuildingBodyUnitTests
{
    // TODO : Sofiane respecte le principe Arrange Act Assert stp

    [Fact]
    public void RecordProperties_ShouldBeSetCorrectly()
    {
        const string builderId = "123", resourceCategory = "Solid", type = "Factory";

        var createBuildingBody = new CreateBuildingBody(builderId, resourceCategory, type);

        Assert.Equal(builderId, createBuildingBody.BuilderId);
        Assert.Equal(resourceCategory, createBuildingBody.ResourceCategory);
        Assert.Equal(type, createBuildingBody.Type);
    }

    [Fact]
    public void RecordEquality_ShouldBeBasedOnProperties()
    {
        var createBuildingBody1 = new CreateBuildingBody("123", "Solid", "Factory");
        var createBuildingBody2 = new CreateBuildingBody("123", "Solid", "Factory");

        Assert.Equal(createBuildingBody1, createBuildingBody2);
    }

    [Fact]
    public void RecordInequality_ShouldBeBasedOnProperties()
    {
        var createBuildingBody1 = new CreateBuildingBody("123", "Solid", "Factory");
        var createBuildingBody2 = new CreateBuildingBody("456", "Liquid", "Mine");

        Assert.NotEqual(createBuildingBody1, createBuildingBody2);
    }
}