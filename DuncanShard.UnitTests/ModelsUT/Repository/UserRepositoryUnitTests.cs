using DuncanShard.Factories;
using DuncanShard.Models;
using DuncanShard.Models.Interfaces;
using DuncanShard.Repository;
using DuncanShard.Services;
using Moq;
using Shard.Shared.Core;

namespace DuncanShard.UnitTests.ModelsUT.Repository;

public class UserRepositoryUnitTests
{
    [Fact]
    public void GetAll_ShouldReturnAllElements()
    {
        var userList = new List<User>
        {
            CreateUser("1", "John"),
            CreateUser("2", "Jane")
        };

        var userRepository = new UserRepository(userList);

        var allUsers = userRepository.GetAll();

        Assert.Equal(userList.Count, allUsers.Count);
    }

    [Fact]
    public void Add_ShouldAddElementToList()
    {
        var userList = new List<User>();
        var userRepository = new UserRepository(userList);
        var newUser = CreateUser("3", "Doe");

        userRepository.Add(newUser);

        Assert.Contains(newUser, userList);
    }

    [Fact]
    public void GetById_ShouldReturnCorrectElement()
    {
        var userList = new List<User>
        {
            CreateUser("1", "John"),
            CreateUser("2", "Jane")
        };

        var userRepository = new UserRepository(userList);

        var user = userRepository.GetById("1");

        Assert.NotNull(user);
        Assert.Equal("1", user!.Id);
        Assert.Equal("John", user.Pseudo);
    }

    [Fact]
    public void GetById_ShouldReturnNullForNonexistentElement()
    {
        var userList = new List<User>
        {
            CreateUser("1", "John"),
            CreateUser("2", "Jane")
        };

        var userRepository = new UserRepository(userList);

        var user = userRepository.GetById("3");

        Assert.Null(user);
    }

    private User CreateUser(string id, string pseudo)
    {
        var clockMock = new Mock<IClock>();
        var mapBuilderServiceMock = new Mock<IMapBuilderService>();
        var combatOrganiserMock = new Mock<ICombatOrganiser>();
        var resourcesQuantity = new Dictionary<Resource, int>();
        var unitFactoryMock = new Mock<UnitFactory>();

        return new User(
            id,
            pseudo,
            mapBuilderServiceMock.Object,
            clockMock.Object,
            combatOrganiserMock.Object,
            DateTime.Now,
            resourcesQuantity,
            true,
            unitFactoryMock.Object
        );
    }
}