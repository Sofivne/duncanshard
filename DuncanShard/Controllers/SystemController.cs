using DuncanShard.Models;
using DuncanShard.Models.DTOs;
using DuncanShard.Models.Mappers;
using DuncanShard.Services;
using Microsoft.AspNetCore.Mvc;

namespace DuncanShard.Controllers
{
    [ApiController]
    [Route("systems")]
    public class SystemController : ControllerBase
    {
        private readonly ILogger<SystemController> _logger;
        private readonly Sector _sector;

        public SystemController(ILogger<SystemController> logger, IMapBuilderService mapGen)
        {
            _logger = logger;
            _sector = mapGen.Sector;
        }

        /// <summary>
        /// API controller returning all systems.
        /// </summary>
        /// <returns>200 status code with a list of all systems.</returns>
        [HttpGet("", Name = "GetAllSystems")]
        public IEnumerable<SystemDto> GetSystems()
        {
            _logger.LogInformation($"{nameof(GetSystems)} | Trying to fetch all systems.");
            return _sector.GetSystems().Select(s => s.ToSystemDto());
        }

        /// <summary>
        /// Returns a system corresponding to the name given if found.
        /// </summary>
        /// <param name="systemName">The system's name to find</param>
        /// <returns>The system found. If not found, returns null.</returns>
        private Models.System? FindSystemByName(string systemName)
        {
            try
            {
                return _sector.GetSystemByName(systemName);
            }
            catch (ArgumentNullException e)
            {
                _logger.LogWarning("In FindSystemByName, systemName is null but shouldn't be");
                return null;
            }
        }


        /// <summary>
        /// API controller returning a system with a given name.
        /// </summary>
        /// <param name="systemName">The system's name to find</param>
        /// <returns>200 status code with the system corresponding to the given name. 404 status code if not found.</returns>
        [HttpGet("{systemName}", Name = "GetASystemByName")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Models.System))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<SystemDto> GetSystem(string systemName)
        {
            _logger.LogInformation($"{nameof(GetSystem)} | Trying to find system with name : {systemName}.");
            var systemFound = FindSystemByName(systemName);
            return systemFound != null ? systemFound.ToSystemDto() : NotFound();
        }
            
        /// <summary>
        /// API controller returning all planets from a system with a given name
        /// </summary>
        /// <param name="systemName">The system's name to find</param>
        /// <returns>200 status code with a list of all planets corresponding to the system found. 404 status code if not found.</returns>
        [HttpGet("{systemName}/planets", Name = "GetPAllPlanetsFromASystem")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<Planet>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<Planet>> GetPlanets(string systemName)
        {
            _logger.LogInformation($"{nameof(GetPlanets)} | Trying to find the planets of system with name : {systemName}.");
            var systemFound = FindSystemByName(systemName);
            return systemFound != null ? systemFound.GetPlanets().ToList() : NotFound();
            // return systemFound != null ? Ok(systemFound.Planets.Select(PlanetToDTO)) : NotFound();
        }
        
        /// <summary>
        /// API controller returning a planet with a given name from a system with a given name
        /// </summary>
        /// <param name="systemName">The system's name to find</param>
        /// <param name="planetName">The planet's name to find</param>
        /// <returns>200 status code with the planet asked for if found. 404 status code if not found.</returns>
        [HttpGet("{systemName}/planets/{planetName}", Name = "GetAPlanetFromASystem")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Planet))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Planet> GetPlanet(string systemName, string planetName)
        {
            _logger.LogInformation($"{nameof(GetPlanet)} | Trying to find planet {planetName} of system with name : {systemName}.");
            
            var systemFound = FindSystemByName(systemName);
            if (systemFound == null)
                return NotFound();
            
            var planetFound = systemFound.GetPlanetByName(planetName);
            // return planetFound != null ? Ok(PlanetToDTO(planetFound)) : NotFound();
            return planetFound != null ? planetFound : NotFound();
        }
        

    }
}
