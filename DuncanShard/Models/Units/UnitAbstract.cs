using DuncanShard.Enums;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Trunks;
using Shard.Shared.Core;

namespace DuncanShard.Models.Units
{
    public abstract class UnitAbstract : IUnit
    {
        public int Health { get; private set; }
        public UnitType[] Priority { get; private set; }
        public string? DestinationShard { get; private set; }
        public void AddToTrunk(Resource resource, int quantity)
        {
            Trunk.AddResource(resource, quantity);
        }

        public int RemoveFromTrunk(Resource resource, int quantity)
        {
            return Trunk.RemoveResource(resource, quantity);
        }

        public int GetResourceQuantityInTrunk(Resource resource)
        {
            return Trunk.GetResourceQuantity(resource);
        }
        
        public void UpdateResourceInTrunk(Resource resource, int quantity)
        {
            Trunk.UpdateQuantity(resource, quantity);
        }

        public int Damage { get; }
        public User Owner { get; }
        public DateTime? EstimatedTimeOfArrival { get; private set; }
        public System? DestinationSystem { get; private set; } // A unit has to be in a system
        public Planet? DestinationPlanet { get; private set; } // A unit can be in a system without being on a planet
        private IList<Task> Tasks { get; set; }
        public ITrunk Trunk { get; }
        
        public string Id { get; }

        public abstract UnitType Type { get; }

        public virtual void GetShotAt(int damage, IUnit attacker)
        {
            if (!CanShoot())
                throw new InvalidOperationException("This unit can't be shot at.");
            Health += damage;
            if (Health <= 0)
                Owner.DeleteUnit(this);
        }

        public System System { get; private set; }

        public Planet? Planet { get; private set; }
        

        
        protected UnitAbstract(System system, Planet? planet, string id, User owner)
        {
            Id = id;
            Tasks = new List<Task>();
            System = system;
            Planet = planet;
            System.WelcomeUnit(this);
            DestinationSystem = system;
            DestinationPlanet = planet;
            EstimatedTimeOfArrival = null;
            Owner = owner;
            Health = 100;
            Priority = Array.Empty<UnitType>();
            Damage = -1;
            Trunk = new NoneTrunk();
        }

        protected UnitAbstract(System system, Planet? planet, string id, User owner, ITrunk trunk, int health = 100) : this(system,
            planet, id, owner)
        {
            Trunk = trunk;
            Health = health;
        }
        
        protected UnitAbstract(System system, Planet? planet, string id, User owner, UnitType[] priority, int damage,
            int health, ICombatOrganiser combatOrganiser) : this(system, planet, id, owner)
        {
            Health = health;
            Priority = priority;
            Damage = damage;
            combatOrganiser.AddAttacker(this);

        }
        
        public async Task MoveUnit(System destinationSystem, Planet? destinationPlanet, IClock clock)
        {
            DestinationSystem = destinationSystem;
            DestinationPlanet = destinationPlanet;
            var estimatedTimeOfArrival = clock.Now;
            if (DestinationSystem != System)
                estimatedTimeOfArrival = estimatedTimeOfArrival.AddMinutes(1);
            if (DestinationPlanet != null)
                estimatedTimeOfArrival = estimatedTimeOfArrival.AddSeconds(15);
            EstimatedTimeOfArrival = estimatedTimeOfArrival;
            Tasks = new List<Task>
            {
                MoveToPlanet(clock),
                MoveToDestinationSystem(clock),
                MoveToPlanet(clock),
            };
            await AwaitArrival();
        }

        public async Task AwaitArrival()
        {
            await Task.WhenAll(Tasks);
            DestinationPlanet = null;
            DestinationSystem = null;
            EstimatedTimeOfArrival = null;
        }
        /// <summary>
        /// Moves the unit to the destination system.
        /// </summary>
        /// <param name="clock">The clock used to await the travel time</param>
        private async Task MoveToDestinationSystem(IClock clock)
        {
            if (DestinationSystem == System || DestinationSystem == null)
                return;
            await clock.Delay(60000);
            System.RemoveUnit(this);
            System = DestinationSystem;
            System.WelcomeUnit(this);
        }
        
        /// <summary>
        /// Moves the unit to the destination planet.
        /// </summary>
        /// <param name="clock">The clock used to await the travel time</param>
        private async Task MoveToPlanet(IClock clock)
        {
            if (DestinationPlanet != null)
            {
                await clock.Delay(15000);
            }
            Planet = DestinationPlanet;
        }

        public bool CanBuild()
        {
            return Type == UnitType.Builder && Planet != null;
        }

        public virtual void Shoot(IUnit target)
        {
            if (!CanShoot())
                return;
            target.GetShotAt(Damage, this);
        }

        /// <summary>
        /// Returns true if the unit can shoot, false otherwise.
        /// </summary>
        /// <returns>true if the unit can shoot, false otherwise.</returns>
        protected virtual bool CanShoot() => false;
    }
}