using System.Collections.Immutable;
using System.Net.Http.Headers;
using System.Text;
using DuncanShard.Enums;
using DuncanShard.Models;
using DuncanShard.Models.DTOs;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Mappers;
using DuncanShard.Services;
using DuncanShard.Utils;
using Microsoft.AspNetCore.Mvc;
using Shard.Shared.Core;

namespace DuncanShard.Controllers
{
    [ApiController]
    [Route("users/{userId}/units")]
    public class UnitController : ControllerBase
    {
        private readonly ILogger<UnitController> _logger;
        private readonly IUserService _userListService;
        private readonly IMapBuilderService _map;
        private readonly IClock _clock;
        private readonly IHttpClientFactory _httpClientFactory;

        public UnitController(ILogger<UnitController> logger, IUserService userListService, IMapBuilderService mapGen,
            IClock clock, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _userListService = userListService;
            _map = mapGen;
            _clock = clock;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Gets all units from a user
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns>200 if user is found, 404 otherwise</returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UnitDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IEnumerable<UnitDto>> GetUnitsOfUser(string userId)
        {
            _logger.LogInformation($"{nameof(GetUnitsOfUser)} | Trying to fetch user's units by user's id");
            var userFound = _userListService.UsersRepository.GetById(userId);
            return userFound == null ? NotFound() : Ok(userFound.UserUnits.Select(unit => unit.ToUnitDto()));
        }

        /// <summary>
        /// Gets a unit from a user by the unit's and user's id
        /// </summary>
        /// <param name="userId">The unit's user's id</param>
        /// <param name="unitId">The unit's id</param>
        /// <returns>The unit if found with a 200, 404 otherwise</returns>
        [HttpGet("{unitId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnitDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UnitDto>> GetUnitById(string userId, string unitId)
        {
            _logger.LogInformation($"{nameof(GetUnitById)} | Trying to fetch user's unit by unit's id");
            var userFound = _userListService.UsersRepository.GetById(userId);

            if (userFound == null)
                return NotFound();

            var unitFound = userFound.GetUnitById(unitId);
            if (unitFound == null)
                return NotFound();


            if (unitFound.EstimatedTimeOfArrival == null)
                return unitFound.ToUnitDto();
            var remainingTime = (TimeSpan) (unitFound.EstimatedTimeOfArrival - _clock.Now);

            if (remainingTime.TotalSeconds <= 2)
                await unitFound.AwaitArrival();
            return unitFound.ToUnitDto();
        }
        
        /// <summary>
        /// Gets a unit's location from a user by the unit's and user's id
        /// </summary>
        /// <param name="userId">The user id</param>
        /// <param name="unitId">The unit id</param>
        /// <returns>The unit's location if found with 200, 404 otherwise</returns>
        [HttpGet("{unitId}/location")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnitLocationDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UnitLocationDto> GetLocation(string userId, string unitId)
        {
            _logger.LogInformation($"{nameof(GetLocation)} | Trying to fetch user's unit location by unit's id");
            var userFound = _userListService.UsersRepository.GetById(userId);

            if (userFound == null)
                return NotFound();

            var unitFound = userFound.GetUnitById(unitId);

            if (unitFound == null)
                return NotFound();

            return unitFound.ToUnitLocationDto();
        }

        /// <summary>
        /// Moves a user's unit to a destination system and planet if there's one given
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <param name="unitId">The unit id</param>
        /// <param name="unitDto">The request body</param>
        /// <returns>The unit in the traveling state</returns>
        [HttpPut("{unitId}", Name = "MoveUnit")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnitDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UnitDto>> UpdateUnit(string userId, string unitId, [FromBody] UnitDto unitDto)
        {
            _logger.LogInformation($"{nameof(UpdateUnit)} | Trying to fetch user's unit by unit's id");

            if (unitDto.Id != unitId)
                return BadRequest("Unit's id must be the same in path parameters and body.");

            var userFound = _userListService.UsersRepository.GetById(userId);

            if (userFound == null)
                return NotFound();

            var unitToUpdate = userFound.GetUnitById(unitId);

            var role = Auth.Auth.GetUserRole(Request);
                
            var parsedResources = unitDto.ResourcesQuantity?
                .Select(kv => new KeyValuePair<Resource, int>(Resource.FromResourceKind(Enum.Parse<ResourceKind>(kv.Key, true)), kv.Value))
                .ToImmutableDictionary(kv => kv.Key, kv => kv.Value);

            if (unitToUpdate is not null && parsedResources is not null 
                                         && !DoesUnitWantToMove(unitToUpdate, unitDto)
                                         && !AreResourcesSame(parsedResources, unitToUpdate.Trunk.ResourcesQuantity))
            {
                try
                {
                    if (unitToUpdate.Type != UnitType.Cargo)
                        throw new InvalidOperationException("Unit is not a cargo unit. Can't load resource in trunk.");
                    if (unitToUpdate.Planet is null || unitToUpdate.Planet.GetBuildings().Any(building => building.Type == BuildingType.Starport.ToString()))
                        throw new InvalidOperationException("Cargo unit is on a planet without starport. Can't load resource in trunk.");
                    ModifyTrunkResource(unitToUpdate, parsedResources, userFound);
                    return unitToUpdate.ToUnitDto();
                }
                catch (InvalidOperationException e)
                {
                    return BadRequest(e.Message);
                }
            }

            if (unitToUpdate == null)
            {
                try
                {
                    return HasCorrectRoleToCreateNewUnit(role)
                           ?? CreateUnit(userFound, unitDto with {System = unitDto.System ?? _map.Wormholes.First().System},
                               parsedResources);
                }
                catch (Exception e)
                {
                    return BadRequest(e.Message);
                }

            }
            var destinationSystem = unitDto.DestinationSystem ?? unitToUpdate.System.Name;

            var newSystem = _map.Sector.GetSystemByName(destinationSystem);

            if (newSystem == null)
                return NotFound("Couldn't find given system.");
            if (!string.IsNullOrEmpty(unitDto.DestinationShard))
                return await TransferToRemoteShard(unitDto, userFound);

            Planet? newPlanet = null;

            if (!string.IsNullOrEmpty(unitDto.DestinationPlanet))
            {
                newPlanet = newSystem.GetPlanetByName(unitDto.DestinationPlanet);
                if (newPlanet == null)
                    return NotFound("Couldn't find given planet in given system.");
            }

            if (unitToUpdate.Planet?.Name == newPlanet?.Name && unitToUpdate.System.Name == newSystem.Name)
                return unitToUpdate.ToUnitDto();

            _ = userFound.MoveUnit(unitToUpdate, newSystem, newPlanet);


            return unitToUpdate.ToUnitDto();
        }

        private async Task<ActionResult<UnitDto>> TransferToRemoteShard(UnitDto unitDto, User userFound)
        {
            var wormhole = _map.Wormholes.FirstOrDefault(w => w.DestinationShard == unitDto.DestinationShard);
            if (wormhole is null)
                return BadRequest("Couldn't find given wormhole.");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri($"{wormhole.BaseUri}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"shard-{wormhole.User}:{wormhole.SharedPassword}")));
            // unitDto.DestinationSystem = wormhole.System;
            var transportUnit = unitDto with {System = wormhole.System, DestinationShard = wormhole.DestinationShard};
            await client.PutAsJsonAsync($"/users/{userFound.Id}/units/{transportUnit.Id}", transportUnit);
            await client.PutAsJsonAsync($"/users/{userFound.Id}", userFound.ToUserDto());
            return RedirectPermanentPreserveMethod($"{wormhole.BaseUri}/users/{userFound.Id}/units/{transportUnit.Id}");
        }

        /// <summary>
        /// Determines if unit wants to move (i.e. Dto unit's destination is different from unit's destination).
        /// </summary>
        /// <param name="unit">The actual unit saved in the system.</param>
        /// <param name="unitDto">The unit from the request.</param>
        /// <returns>True if unit wants to move, false otherwise.</returns>
        private bool DoesUnitWantToMove(IUnit unit, UnitDto unitDto)
        {
            return unit.DestinationPlanet?.Name != unitDto.DestinationPlanet ||
                   unit.DestinationSystem?.Name != unitDto.DestinationSystem;
        }

        private void ModifyTrunkResource(IUnit unitToUpdate, ImmutableDictionary<Resource, int> resources, User user)
        {
            resources
                // .Select(kv => new KeyValuePair<Resource, int>(Resource.FromResourceKind(Enum.Parse<ResourceKind>(kv.Key, true)), kv.Value))
                .ToList()
                .ForEach(resource =>
                {
                    var actualResourceQtyInUnit = unitToUpdate.GetResourceQuantityInTrunk(resource.Key);
                    var resourceQtyToRemoveOrAdd = resource.Value - actualResourceQtyInUnit;

                    if (resourceQtyToRemoveOrAdd > 0 && resourceQtyToRemoveOrAdd + actualResourceQtyInUnit >  user.GetQuantityOfResource(resource.Key))
                        throw new InvalidOperationException("Trying to put more resource in trunk cargo than user has.");
                    
                    unitToUpdate.UpdateResourceInTrunk(resource.Key, resource.Value);
                    if (resourceQtyToRemoveOrAdd <= 0)
                    {
                        // unitToUpdate.RemoveFromTrunk(resource.Key, resourceQtyToRemoveOrAdd);
                        user.AddResource(resource.Key, -resourceQtyToRemoveOrAdd);
                        return;
                    }
                    // unitToUpdate.AddToTrunk(resource.Key, resourceQtyToRemoveOrAdd);
                    user.RemoveResource(resource.Key, resourceQtyToRemoveOrAdd);
                });
        }
        
        /// <summary>
        /// Determines if two dictionaries of resources are the same.
        /// </summary>
        /// <param name="dictionary1">Dictionary to compare with second one.</param>
        /// <param name="dictionary2">Dictionary to compare with first one.</param>
        /// <returns>True if dictionaries are equals, false otherwise.</returns>
        private bool AreResourcesSame(IDictionary<Resource, int> dictionary1, IDictionary<Resource, int> dictionary2)
        {
            if (dictionary1.Count != dictionary2.Count)
                return false;
            foreach (var (key, value) in dictionary1)
                if (!dictionary2.ContainsKey(key) || dictionary2[key] != value)
                    return false;
            return true;
        }
        
        /// <summary>
        /// Returns whether or not the user has the correct role to create a new unit.
        /// </summary>
        /// <param name="role">Client's role.</param>
        /// <returns>Null if user has role and an error otherwise.</returns>
        private ActionResult? HasCorrectRoleToCreateNewUnit(Role role)
        {
            var roles = new List<Role> {Role.Admin, Role.Shard};
            if (role == Role.NotAuthenticated)
                return Unauthorized("You must be logged in to create a unit.");
            return roles.Contains(role) ? null : Forbid("Can't create a unit without correct role.");
        }

        /// <summary>
        /// Creates a unit.
        /// </summary>
        /// <param name="userFound">User we want to add the unit to.</param>
        /// <param name="unitDto">The unit dto to create from a unit.</param>
        /// <param name="resources">The resource to add in the trunk of a cargo.</param>
        /// <returns>The unit created or a 400 response if unit's type is wrong.</returns>
        private ActionResult<UnitDto> CreateUnit(User userFound, UnitDto unitDto, IDictionary<Resource, int>? resources)
        {
            _logger.LogInformation($"{nameof(CreateUnit)} | Trying to create a unit.");
            if (unitDto.System == null)
                return BadRequest("Unit's system is null.");
            
            var system = _map.Sector.GetSystemByName(unitDto.System);
            if (system == null)
                return NotFound("Couldn't find given system.");
            
            var planet = unitDto.Planet is null ? null : system.GetPlanetByName(unitDto.Planet);
            
            var isUnitTypeCorrect = Enum.TryParse(unitDto.Type, true, out UnitType unitType);
            if (!isUnitTypeCorrect)
                return BadRequest("Unit's type is incorrect.");
            
            _logger.LogInformation($"{nameof(CreateUnit)} | Creating a unit.");
            return resources is not null 
                ? userFound.AddUnit(system, planet, unitType, unitDto.Health, resources, unitDto.Id).ToUnitDto() 
                : userFound.AddUnit(system, planet, unitType, unitDto.Id).ToUnitDto();
        }
    }
}