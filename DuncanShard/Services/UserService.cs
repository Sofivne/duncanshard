using DuncanShard.Models;
using DuncanShard.Repository;

namespace DuncanShard.Services
{
    public class UserService : IUserService
    {
        public IRepository<User> UsersRepository { get; } = new UserRepository(new List<User>());
    }
}
