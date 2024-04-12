using DuncanShard.Models;
using DuncanShard.Services;
using System;
using System.Collections.Generic;
using DuncanShard.Enums;
using DuncanShard.Factories;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Units;
using Microsoft.Extensions.Options;
using Shard.Shared.Core;
using Xunit;

namespace DuncanShard.UnitTests.ModelsUT
{
    public class UsersUnitTests
    {
        /*
         * Test pour la création d'un objet User avec des paramètres valides
         */
        [Fact]
        public void User_Object_Created_Successfully_With_Valid_Parameters()
        {
            const string id = "abc123";
            const string pseudo = "JohnDoe";
            IMapBuilderService map = new MapBuilderService(
                Options.Create(new MapGeneratorOptions { Seed = "Test application" }),
                new BuildingFactory());
            IClock clock = new SystemClock();
            ICombatOrganiser combatOrganiser = new CombatOrganiser(clock);
            Dictionary<Resource, int> resources = new Dictionary<Resource, int>
            {
                { Resource.FromResourceKind(ResourceKind.Carbon), 20 },
                { Resource.FromResourceKind(ResourceKind.Iron), 10 },
                { Resource.FromResourceKind(ResourceKind.Oxygen), 50 },
                { Resource.FromResourceKind(ResourceKind.Water), 50 },
                { Resource.FromResourceKind(ResourceKind.Aluminium), 0 },
                { Resource.FromResourceKind(ResourceKind.Titanium), 0 },
                { Resource.FromResourceKind(ResourceKind.Gold), 0 },
            };
            UnitFactory unitFactory = new UnitFactory();

            User user = new User(id, pseudo, map, clock, combatOrganiser, DateTime.Now, resources, true, unitFactory);

            Assert.Equal(id, user.Id);
            Assert.Equal(pseudo, user.Pseudo);
            Assert.NotNull(user.UserUnits);
            Assert.NotNull(user.DateOfCreation);
            Assert.NotEmpty(user.SafeResourcesQuantity);
        }
        
        /*
         * Test pour la création d'un objet User avec un id invalide
         */
        [Fact]
        public void User_Object_Creation_Fails_With_Invalid_Id()
        {
            const string id = "abc123@";
            const string pseudo = "JohnDoe";
            IMapBuilderService map = new MapBuilderService(
                Options.Create(new MapGeneratorOptions { Seed = "Test application" }),
                new BuildingFactory());
            IClock clock = new SystemClock();
            ICombatOrganiser combatOrganiser = new CombatOrganiser(clock);
            Dictionary<Resource, int> resources = new Dictionary<Resource, int>
            {
                { Resource.FromResourceKind(ResourceKind.Carbon), 20 },
                { Resource.FromResourceKind(ResourceKind.Iron), 10 },
                { Resource.FromResourceKind(ResourceKind.Oxygen), 50 },
                { Resource.FromResourceKind(ResourceKind.Water), 50 },
                { Resource.FromResourceKind(ResourceKind.Aluminium), 0 },
                { Resource.FromResourceKind(ResourceKind.Titanium), 0 },
                { Resource.FromResourceKind(ResourceKind.Gold), 0 },
            };
            UnitFactory unitFactory = new UnitFactory();

            Assert.Throws<ArgumentException>(() => new User(id, pseudo, map, clock, combatOrganiser, DateTime.Now, resources, true, unitFactory));
        }
        
        /*
         * Test pour l'ajout d'une unité à un joueur
         */
         [Fact]
        public void Add_Unit_To_User_Units_List()
        {
            const string id = "abc123";
            const string pseudo = "JohnDoe";
            IMapBuilderService map = new MapBuilderService(
                Options.Create(new MapGeneratorOptions { Seed = "Test application" }),
                new BuildingFactory());
            IClock clock = new SystemClock();
            ICombatOrganiser combatOrganiser = new CombatOrganiser(clock);
            Dictionary<Resource, int> resources = new Dictionary<Resource, int>
            {
                { Resource.FromResourceKind(ResourceKind.Carbon), 20 },
                { Resource.FromResourceKind(ResourceKind.Iron), 10 },
                { Resource.FromResourceKind(ResourceKind.Oxygen), 50 },
                { Resource.FromResourceKind(ResourceKind.Water), 50 },
                { Resource.FromResourceKind(ResourceKind.Aluminium), 0 },
                { Resource.FromResourceKind(ResourceKind.Titanium), 0 },
                { Resource.FromResourceKind(ResourceKind.Gold), 0 },
            };
            UnitFactory unitFactory = new UnitFactory();

            User user = new User(id, pseudo, map, clock, combatOrganiser, DateTime.Now, resources, true, unitFactory);
            var initialUnitCount = user.UserUnits.Count();
            user.AddUnit(map.GetRandomSystem(), null, UnitType.Scout);
            var updatedUnitCount = user.UserUnits.Count();

            Assert.Equal(initialUnitCount + 1, updatedUnitCount);
        }

        /*
         * Test pour la suppression d'une unité d'un joueur
         */
        [Fact]
        public void Remove_Unit_From_User_Units_List()
        {
            const string id = "abc123";
            const string pseudo = "JohnDoe";
            IMapBuilderService map = new MapBuilderService(
                Options.Create(new MapGeneratorOptions { Seed = "Test application" }),
                new BuildingFactory());
            IClock clock = new SystemClock();
            ICombatOrganiser combatOrganiser = new CombatOrganiser(clock);
            Dictionary<Resource, int> resources = new Dictionary<Resource, int>
            {
                { Resource.FromResourceKind(ResourceKind.Carbon), 20 },
                { Resource.FromResourceKind(ResourceKind.Iron), 10 },
                { Resource.FromResourceKind(ResourceKind.Oxygen), 50 },
                { Resource.FromResourceKind(ResourceKind.Water), 50 },
                { Resource.FromResourceKind(ResourceKind.Aluminium), 0 },
                { Resource.FromResourceKind(ResourceKind.Titanium), 0 },
                { Resource.FromResourceKind(ResourceKind.Gold), 0 },
            };
            UnitFactory unitFactory = new UnitFactory();

            User user = new User(id, pseudo, map, clock, combatOrganiser, DateTime.Now, resources, true, unitFactory);

            var unitToRemove = user.UserUnits.First();
            user.DeleteUnit(unitToRemove);

            Assert.DoesNotContain(unitToRemove, user.UserUnits);
        }

        
        /*
         * Test pour l'ajout d'une quantité supplémentaire à une ressources d'un joueur
         */
        [Fact]
        public void Add_Quantity_Resource_To_User_Resources()
        {
            const string id = "abc123";
            const string pseudo = "JohnDoe";
            IMapBuilderService map = new MapBuilderService(Options.Create(new MapGeneratorOptions { Seed = "Test application" }), new BuildingFactory());
            IClock clock = new SystemClock();
            ICombatOrganiser combatOrganiser = new CombatOrganiser(clock); 
            Dictionary<Resource, int> resources = new Dictionary<Resource, int>
            {
                { Resource.FromResourceKind(ResourceKind.Carbon), 20 },
                { Resource.FromResourceKind(ResourceKind.Iron), 10 },
                { Resource.FromResourceKind(ResourceKind.Oxygen), 50 },
                { Resource.FromResourceKind(ResourceKind.Water), 50 },
                { Resource.FromResourceKind(ResourceKind.Aluminium), 0 },
                { Resource.FromResourceKind(ResourceKind.Titanium), 0 },
                { Resource.FromResourceKind(ResourceKind.Gold), 0 },
            };
            UnitFactory unitFactory = new UnitFactory();

            User user = new User(id, pseudo, map, clock, combatOrganiser, DateTime.Now, resources, true, unitFactory);
            var resourceToAdd = Resource.FromResourceKind(ResourceKind.Carbon);

            user.AddResource(resourceToAdd, 1);

            Assert.Equal(21, user.SafeResourcesQuantity[resourceToAdd]);
        }

        /*
         * Test pour la suppression d'une quantité d'une ressources d'un joueur
         */
        [Fact]
        public void Remove_Resource_Quantity_From_User_Resources()
        {
            const string id = "abc123";
            const string pseudo = "JohnDoe";
            IMapBuilderService map = new MapBuilderService(Options.Create(new MapGeneratorOptions { Seed = "Test application" }), new BuildingFactory());
            IClock clock = new SystemClock();
            ICombatOrganiser combatOrganiser = new CombatOrganiser(clock); 
            Dictionary<Resource, int> resources = new Dictionary<Resource, int>
            {
                { Resource.FromResourceKind(ResourceKind.Carbon), 20 },
                { Resource.FromResourceKind(ResourceKind.Iron), 10 },
                { Resource.FromResourceKind(ResourceKind.Oxygen), 50 },
                { Resource.FromResourceKind(ResourceKind.Water), 50 },
                { Resource.FromResourceKind(ResourceKind.Aluminium), 0 },
                { Resource.FromResourceKind(ResourceKind.Titanium), 0 },
                { Resource.FromResourceKind(ResourceKind.Gold), 0 },
            };
            UnitFactory unitFactory = new UnitFactory();

            User user = new User(id, pseudo, map, clock, combatOrganiser, DateTime.Now, resources, true, unitFactory);
            var resourceToRemove = Resource.FromResourceKind(ResourceKind.Carbon);
            user.RemoveResource(resourceToRemove, 1);
         
            Assert.Equal(19, user.SafeResourcesQuantity[resourceToRemove]);
        }
        
    }
}
