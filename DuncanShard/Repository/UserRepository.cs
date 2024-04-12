using DuncanShard.Models;

namespace DuncanShard.Repository;

public class UserRepository : Repository<User>
{
    public UserRepository(IList<User> userList) : base(userList) // équivalent super()
    {
    }

    public override User? GetById(string id) => List.FirstOrDefault(user => user.Id == id);
}