using DuncanShard.Models;

namespace DuncanShard.Repository;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly IList<T> List;

    protected Repository(IList<T> list)
    {
        List = list;
    }

    public abstract T? GetById(string id);

    public IList<T> GetAll()
    {
        return List;
    }

    public void Add(T elementToAdd)
    {
        List.Add(elementToAdd);
    }
}