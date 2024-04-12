using System.Collections.Immutable;
using System.Text.Json.Serialization;
using DuncanShard.Factories;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Units;
using Shard.Shared.Core;

namespace DuncanShard.Models
{
    public class System
    {
        // private SystemSpecification SystemSpecification { get; }
        public string Name { get; private init; }
        private List<Planet> Planets { get; init; }
        
        /// <summary>
        /// Returns all the planets in the system.
        /// </summary>
        /// <returns>The system's planets.</returns>
        public IEnumerable<Planet> GetPlanets() => Planets.ToImmutableArray();
        
        /// <summary>
        /// Returns a planet corresponding to the name given if found.
        /// </summary>
        /// <param name="planetName">The planet to find.</param>
        /// <returns>The planet if found, null otherwise.</returns>
        public Planet? GetPlanetByName(string planetName) =>
            Planets.FirstOrDefault(planet => planet.Name == planetName);
        
        private IList<IUnit> Units { get; } = new List<IUnit>();

        /// <summary>
        /// The constructor for the System class
        /// </summary>
        /// <param name="systemSpecification">The System type from the core project</param>
        public System(SystemSpecification systemSpecification, BuildingFactory buildingFactory)
        {
            Name = systemSpecification.Name;
            Planets = systemSpecification.Planets
                .Select(planetSpecification => new Planet(planetSpecification, buildingFactory)).ToList();
        }

        /// <summary>
        /// Adds a unit to the system.
        /// </summary>
        /// <param name="unit">The unit to add.</param>
        public void WelcomeUnit(IUnit unit)
        {
            Units.Add(unit);
        }
        
        public IEnumerable<IUnit> GetUnits() => Units.ToImmutableArray();

        /// <summary>
        /// Removes a unit from the system.
        /// </summary>
        /// <param name="unit">The unit to remove.</param>
        public void RemoveUnit(IUnit unit)
        {
            Units.Remove(unit);
        }
    }
}