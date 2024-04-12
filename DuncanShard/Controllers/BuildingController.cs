using DuncanShard.Enums;
using DuncanShard.Models;
using DuncanShard.Models.DTOs;
using DuncanShard.Models.Mappers;
using DuncanShard.Models.RequestBodies;
using DuncanShard.Services;
using Microsoft.AspNetCore.Mvc;
using Shard.Shared.Core;

namespace DuncanShard.Controllers
{
    [ApiController]
    [Route("users/{userId}/buildings")]
    public class BuildingController : ControllerBase
    {
        private readonly ILogger<SystemController> _logger;
        private readonly Sector _sector;
        private readonly IUserService _userListService;
        private readonly IClock _clock;

        public BuildingController(ILogger<SystemController> logger, IMapBuilderService mapGen, IUserService userListService, IClock clock)
        {
            _logger = logger;
            _sector = mapGen.Sector;
            _userListService = userListService;
            _clock = clock;
        }
        
        /// <summary>
        /// API controller creating a building.
        /// </summary>
        /// <param name="userId">The user's id to create a building on.</param>
        /// <param name="buildingBody">The building to be created.</param>
        /// <returns></returns>
        // TODO : Remove nullable from building
        [HttpPost("", Name = "CreateBuilding")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<BuildingDto> CreateBuilding(string userId, [FromBody] CreateBuildingBody buildingBody)
        {
            _logger.LogInformation($"{nameof(CreateBuilding)} | Trying to create building.");
            
            if (!Enum.TryParse(buildingBody.Type, true, out BuildingType buildingType))
                return BadRequest("Building type is unknown");
            
            var isResourceCategoryParsed = Enum.TryParse<ResourcesCategory>(buildingBody.ResourceCategory, true, out var resourcesCategory);
            
            if (!isResourceCategoryParsed && buildingType == BuildingType.Mine)
                return BadRequest("Mine must have a resource category");
            
            if (string.IsNullOrWhiteSpace(buildingBody.BuilderId))
                return BadRequest("BuilderId is missing");
            
            var user = _userListService.UsersRepository.GetById(userId);

            if (user == null)
                return NotFound("Couldn't find user");

            var builder = user.GetUnitById(buildingBody.BuilderId);
            if (builder == null)
                return BadRequest("Couldn't find builder");

            if (builder.Planet == null)
                return BadRequest("Builder is not on a planet");

            if (!builder.CanBuild())
                return BadRequest("Builder can't build without being on a planet.");
            
            var planet = _sector.GetSystems()
                .SelectMany(system => system.GetPlanets())
                .FirstOrDefault(p => p.Name == builder.Planet?.Name);
            if (planet == null)
                return NotFound("Couldn't find planet");
            
            try
            {
                var building = planet.CreateBuilding(buildingType, builder, resourcesCategory, _clock,
                    user);
                user.AddBuilding(building);
                return building.ToBuildingDto();
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Building type is wrong");
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

        }

        /// <summary>
        /// API controller return all buildings of a user.
        /// </summary>
        /// <param name="userId">The user's id to get buildings from.</param>
        /// <returns></returns>
        [HttpGet("", Name = "GetBuildings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<IList<BuildingDto>> GetBuildings(string userId)
        {
            _logger.LogInformation($"{nameof(GetBuildings)} | Trying to get buildings.");
            var user = _userListService.UsersRepository.GetById(userId);
            if (user == null)
                return NotFound("Couldn't find user");
            
            var buildings = user.UserBuildings.Select(building => building.ToBuildingDto()).ToList();
            return buildings;
        }
        
        
        /// <summary>
        /// Creates a unit from a starport.
        /// </summary>
        /// <param name="userId">The user we want to add the unit to.</param>
        /// <param name="buildingId">The starport's id.</param>
        /// <param name="queueDto">Type of unit to create.</param>
        /// <returns></returns>
        [HttpPost("{buildingId}/queue")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UnitDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UnitDto> QueueUnit(string userId, string buildingId, [FromBody] QueueDto queueDto)
        {
            _logger.LogInformation($"{nameof(QueueUnit)} | Queueing an units.");
            var userFound = _userListService.UsersRepository.GetById(userId);

            if (userFound == null)
                return NotFound();

            var buildingFound = userFound.GetBuildingById(buildingId);
            if (buildingFound == null)
                return NotFound();
            if (buildingFound.Type != BuildingType.Starport.ToString().ToLower())
                return BadRequest("Can only build units from a starport.");
            if(!buildingFound.IsBuilt)
                return BadRequest("Building is not built.");
            if (!Enum.TryParse<UnitType>(queueDto.Type, true, out var unitType))
                return BadRequest("Building type is wrong.");
            try
            {
                return buildingFound.Use(unitType)!.ToUnitDto();
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(e.Message);
            }
        }
        
        /// <summary>
        /// API controller return a building of a user by Building id.
        /// </summary>
        /// <param name="userId">The user's id to get buildings from.</param>
        /// <param name="buildingId">The building's id to get Building from</param>
        /// <returns></returns>
        [HttpGet("{buildingId}", Name = "GetBuilding")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<BuildingDto>> GetBuilding(string userId, string buildingId)
        {
            _logger.LogInformation($"{nameof(GetBuilding)} | Trying to get building.");
            var user = _userListService.UsersRepository.GetById(userId);
            if (user == null)
                return NotFound("Couldn't find user");
            
            var building = user.GetBuildingById(buildingId);
            if (building == null)
                return NotFound("Couldn't find building");
            
            if (building.EstimatedBuildTime == null)
                return building.ToBuildingDto();
            
            var remainingTime = (TimeSpan)(building.EstimatedBuildTime - _clock.Now);
            if (remainingTime.TotalSeconds > 2)
                return building.ToBuildingDto();
            

            try
            {
                await building.AwaitBuilding();
                return building.ToBuildingDto();
            }
            catch (OperationCanceledException)
            {
                
            }
            await building.Builder.AwaitArrival();
            return NotFound("Not found");
        }
        
    }
    
    
}