using System.Collections.Immutable;
using System.Text.RegularExpressions;
using DuncanShard.Enums;
using DuncanShard.Factories;
using DuncanShard.Models.Interfaces;
using DuncanShard.Services;
using Shard.Shared.Core;

namespace DuncanShard.Models
{
    public partial class User
    {
        private readonly IClock _clock;
        private readonly UnitFactory _unitFactory;

        public User(string id, string pseudo, IMapBuilderService map, IClock clock, ICombatOrganiser combatOrganiser, DateTime? dateOfCreation, UnitFactory unitFactory)
            : this(id, pseudo, map, clock, combatOrganiser, dateOfCreation, DefaultResourceQuantity(), true, unitFactory)
        {}

        public User(string id, string pseudo, IMapBuilderService map, IClock clock, ICombatOrganiser combatOrganiser, DateTime? dateOfCreation,
            Dictionary<Resource, int> resourcesQuantity, bool shouldCreateUnits, UnitFactory unitFactory)
        {
            if (!IsIdCorrect(id))
                throw new ArgumentException("Id must respect following regex : '^[a-zA-Z0-9_-]+$'");

            Id = id;
            Pseudo = pseudo;
            var system = map.GetRandomSystem();
            Units = new List<IUnit>();
            _clock = clock;
            DateOfCreation = dateOfCreation ?? _clock.Now;
            Buildings = new List<IBuilding>();
            ResourcesQuantity = resourcesQuantity;
            CombatOrganiser = combatOrganiser;
            _unitFactory = unitFactory;
            if (!shouldCreateUnits) return;
            AddUnit(system, null, UnitType.Scout);
            AddUnit(system, null, UnitType.Builder);
        }
        
        private ICombatOrganiser CombatOrganiser { get; }

        /// <summary>
        /// Adds a building to the user's buildings list.
        /// </summary>
        /// <param name="building">The building to add.</param>
        public void AddBuilding(IBuilding building)
        {
            Buildings.Add(building);
        }

        /// <summary>
        /// Deletes a unit from the user's units list.
        /// </summary>
        /// <param name="unit">The unit to delete.</param>
        public void DeleteUnit(IUnit unit)
        {
            Units.Remove(unit);
            unit.System.RemoveUnit(unit);
        }

        /// <summary>
        /// Creates and adds a unit to the user's units list. Different from the other AddUnit method because it allows to add resources and an health to the unit.
        /// </summary>
        /// <param name="system">The system the unit is on.</param>
        /// <param name="planet">The planet the unit is on (null is unit isn't on a planet).</param>
        /// <param name="unitType">The unit's type.</param>
        /// <param name="resources">Unit's resources that will be added to the unit.</param>
        /// <param name="id">Unit's id if already generated. Null for automatic generation of the id.</param>
        /// <param name="health">Unit's health.</param>
        /// <returns>The created unit.</returns>
        public IUnit AddUnit(System system, Planet? planet, UnitType unitType, int health,
            IDictionary<Resource, int> resources, string? id = null)
        {
            var unit = _unitFactory.CreateUnit(system, planet, unitType, this, _clock, CombatOrganiser, id, health, resources);
            Units.Add(unit);
            return unit;
        }

        /// <summary>
        /// Creates and adds a unit to the user's units list.
        /// </summary>
        /// <param name="system">The system the unit is on.</param>
        /// <param name="planet">The planet the unit is on (null is unit isn't on a planet).</param>
        /// <param name="unitType">The unit's type.</param>
        /// <param name="id">Unit's id if already generated. Null for automatic generation of the id.</param>
        /// <returns>The created unit.</returns>
        public IUnit AddUnit(System system, Planet? planet, UnitType unitType, string? id = null)
        {
            var unit = _unitFactory.CreateUnit(system, planet, unitType, this, _clock, CombatOrganiser, id);
            Units.Add(unit);
            return unit;
        }

        /// <summary>
        /// Used to check if user's id respects a given format.
        /// Format is described with this next regex : ^[a-zA-Z0-9_-]+$
        /// </summary>
        /// <param name="id">Id to check</param>
        /// <returns>True if id is correct, false otherwise</returns>
        private bool IsIdCorrect(string id) =>
            MyRegex().IsMatch(id);

        public string Id { get; }

        public string Pseudo { get; }

        public DateTime DateOfCreation { get; private init; }
        private IList<IUnit> Units { get; }
        private IList<IBuilding> Buildings { get; }
        private Dictionary<Resource, int> ResourcesQuantity { get; }
        public ImmutableDictionary<Resource, int> SafeResourcesQuantity => ResourcesQuantity.ToImmutableDictionary();
        
        public IUnit? GetUnitById(string unitId) => Units.FirstOrDefault(unit => unit.Id == unitId);
        public IEnumerable<IUnit> UserUnits => Units.ToList();
        
        public IBuilding? GetBuildingById(string buildingId) => Buildings.FirstOrDefault(building => building.Id == buildingId);
        public IEnumerable<IBuilding> UserBuildings => Buildings.ToList();
        
        /// <summary>
        /// Removes a given quantity of a given resource. If quantity is greater than actual quantity of the user's resource, actual quantity is set to 0.
        /// </summary>
        /// <param name="resource">The resource to remove.</param>
        /// <param name="quantity">The quantity to remove.</param>
        /// <returns>The new actual resource's quantity.</returns>
        public int RemoveResource(Resource resource, int quantity)
        {
            if (!ResourcesQuantity.ContainsKey(resource))
                return 0;
            var actualQuantity = ResourcesQuantity[resource];
            if (actualQuantity <= quantity)
            {
                ResourcesQuantity[resource] = 0;
                return 0;
            }
            ResourcesQuantity[resource] -= quantity;
            return ResourcesQuantity[resource];
        }
        
        /// <summary>
        /// Updates the quantity of a given resource (just like a setter).
        /// </summary>
        /// <param name="resource">The resource to update.</param>
        /// <param name="quantity">The new quantity.</param>
        public void UpdateResource(Resource resource, int quantity)
        {
            ResourcesQuantity[resource] = quantity;
        }
        
        /// <summary>
        /// Adds a given quantity of a given resource.
        /// </summary>
        /// <param name="resource">The resource to add.</param>
        /// <param name="quantity">The quantity to add.</param>
        public void AddResource(Resource resource, int quantity)
        {
            if (ResourcesQuantity.ContainsKey(resource))
                ResourcesQuantity[resource] += quantity;
            else
                ResourcesQuantity.Add(resource, quantity);
        }

        /// <summary>
        /// Returns the quantity of a given resource.
        /// </summary>
        /// <param name="resource">The resource to fetch.</param>
        /// <returns>0 if resource isn't present, the quantity otherwise.</returns>
        public int GetQuantityOfResource(Resource resource)
        {
            var hasResource = ResourcesQuantity.TryGetValue(resource, out var resourceQty);
            return hasResource ? resourceQty : 0;
        }

        /// <summary>
        /// Returns the default quantity of resources for a new user.
        /// </summary>
        /// <returns>The default quantity of resources for a new user.</returns>
        public static Dictionary<Resource, int> DefaultResourceQuantity() =>
            new()
            {
                { Resource.FromResourceKind(ResourceKind.Carbon), 20 },
                { Resource.FromResourceKind(ResourceKind.Iron), 10 },
                { Resource.FromResourceKind(ResourceKind.Oxygen), 50 },
                { Resource.FromResourceKind(ResourceKind.Water), 50 },
                { Resource.FromResourceKind(ResourceKind.Aluminium), 0 },
                { Resource.FromResourceKind(ResourceKind.Titanium), 0 },
                { Resource.FromResourceKind(ResourceKind.Gold), 0 },
            };

        /// <summary>
        /// Adds a resource to the user's resources list.
        /// </summary>
        /// <param name="resourceKind">The resource to add to.</param>
        public void AddResource(Resource resourceKind)
        {
            var containsResource = ResourcesQuantity.TryGetValue(resourceKind, out var resourceQty);
            if (!containsResource) 
                resourceQty = 0;
            ResourcesQuantity[resourceKind] = resourceQty + 1;
        }

        /// <summary>
        /// Moves a unit to a new system and planet.
        /// </summary>
        /// <param name="unit">The unit to move.</param>
        /// <param name="newSystem">The system to move the unit to.</param>
        /// <param name="newPlanet">The planet to move the unit to.</param>
        public async Task MoveUnit(IUnit unit, System newSystem, Planet? newPlanet)
        {
            // var unit = Units.First(u => u.Id == unitToUpdate.Id);
            if (unit.Planet == null)
            {
                await unit.MoveUnit(newSystem, newPlanet, _clock);
                return;
            }
            
            var buildingsToRemove = unit.Planet.GetBuildings()
                .Where(building => building.Builder.Id == unit.Id && building.EstimatedBuildTime != null)
                .ToList();
            foreach (var building in buildingsToRemove)
            {
                Buildings.Remove(building);
                // unit.Planet.Buildings.Remove(building);
                unit.Planet.DestroyBuilding(building);
                building.CancelBuilding();
            }
            
            await unit.MoveUnit(newSystem, newPlanet, _clock);
            
            
        }

        [GeneratedRegex("^[a-zA-Z0-9_-]+$")]
        private static partial Regex MyRegex();
    }
}