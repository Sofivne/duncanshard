using DuncanShard.Models;
using DuncanShard.Repository;

namespace DuncanShard.Services;

public interface IUserService
{
    public IRepository<User> UsersRepository { get; }
}