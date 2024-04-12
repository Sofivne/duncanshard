using DuncanShard.Factories;
using DuncanShard.Models;
using DuncanShard.Models.DTOs;
using DuncanShard.Models.Interfaces;
using DuncanShard.Models.Mappers;
using DuncanShard.Models.Units;
using DuncanShard.Services;
using DuncanShard.Utils;
using Microsoft.AspNetCore.Mvc;
using Shard.Shared.Core;


namespace DuncanShard.Controllers
{
    [ApiController]
    [Route("users/{id}")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userListService;
        private readonly IMapBuilderService _map;
        private readonly IClock _clock;
        private readonly ICombatOrganiser _combatOrganiser;
        private readonly UnitFactory UnitFactory;
        public UserController(ILogger<UserController> logger, IUserService userListService, IMapBuilderService mapGen, IClock clock, ICombatOrganiser combatOrganiser, UnitFactory unitFactory)
        {
            _clock = clock;
            _logger = logger;
            _userListService = userListService;
            _map = mapGen;
            _combatOrganiser = combatOrganiser;
            UnitFactory = unitFactory;
        }

        /// <summary>
        /// Creates a user with a given id and pseudo
        /// </summary>
        /// <param name="id">The user's id</param>
        /// <param name="userDto">The request body</param>
        /// <returns>The user created</returns>
        [HttpPut("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDto> NewUser(string id, [FromBody] UserDto userDto)
        {
            _logger.LogInformation($"{nameof(NewUser)} | Trying to create a user.");
            if (userDto.Id != id)
                return BadRequest("Id in body and Id in path parameter are not equal.");
            var role = Auth.Auth.GetUserRole(Request);

            var userFound = _userListService.UsersRepository.GetById(id);
            if (userFound != null && role == Role.Admin)
            {
                if (_updateUserResource(userDto, userFound))
                    return Ok(userFound.ToUserDto());
                return BadRequest("Couldn't update user's resources.");
            }

            User user;
            try
            {
                user = CreateUser(userDto, role == Role.Shard);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

            _logger.LogInformation($"NewUser | Trying to put new user : {user}.");
            _userListService.UsersRepository.Add(user);

            return user.ToUserDto();
        }
        
        /// <summary>
        /// Creates a new user. Can be an original new user or a transfer from another shard.
        /// </summary>
        /// <param name="userDto">The user to create.</param>
        /// <param name="isShardTransfer">True if user creation is actually a transfer from another shard, false otherwise.</param>
        /// <returns>The created user.</returns>
        private User CreateUser(UserDto userDto, bool isShardTransfer)
        {
            if (!isShardTransfer)
                return new User(userDto.Id, userDto.Pseudo, _map, _clock, _combatOrganiser, null, UnitFactory);
            var resources = Models.User.DefaultResourceQuantity().ToDictionary(v => v.Key, _ => 0);
            return new User(userDto.Id, userDto.Pseudo, _map, _clock, _combatOrganiser, userDto.DateOfCreation, resources, false, UnitFactory);
        }

        /// <summary>
        /// Updates user's resources.
        /// </summary>
        /// <param name="userDto"></param>
        /// <param name="user"></param>
        /// <returns>True if update has been done, false otherwise.</returns>
        private bool _updateUserResource(UserDto userDto, User user)
        {
            if (userDto.ResourcesQuantity == null)
                return false;
            var resources = userDto.ResourcesQuantity // TODO : check les ressources sont valides
                .Select(resource => new KeyValuePair<Resource, int>
                    (Resource.FromResourceKind(Enum.Parse<ResourceKind>(resource.Key, true)), resource.Value)
                );
                // .ToList()
                // .ForEach(kv => user.RemoveResource(kv.Key, kv.Value));
            foreach (var resource in resources)
                user.UpdateResource(resource.Key, resource.Value);
            return true;
        }

        /// <summary>
        /// Gets a user by its id
        /// </summary>
        /// <param name="id">The user's id</param>
        /// <returns>200 with the user, 404 if not found</returns>
        [HttpGet("")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<UserDto> GetUserById(string id)
        {
            _logger.LogInformation($"{nameof(GetUserById)} | Trying to fetch a user by id");
            var userFound = _userListService.UsersRepository.GetById(id);

            if (userFound == null)
            {
                return NotFound();
            }

            return userFound.ToUserDto();
        }
    }
}