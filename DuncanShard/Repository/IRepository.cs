using DuncanShard.Models;

namespace DuncanShard.Repository;

public interface IRepository<T>
{
    /// <summary>
    /// Retrieves a T object stored corresponding to the given id.
    /// </summary>
    /// <param name="id">A string id</param>
    /// <returns>The object with the given id or null if not found</returns>
    T? GetById(string id);
    /// <summary>
    /// Returns a list of all T objects stored.
    /// </summary>
    /// <returns>A list of all T object stored</returns>
    IList<T> GetAll();
    /// <summary>
    /// Adds and stores a new element.
    /// </summary>
    /// <param name="elementToAdd">The new element to add</param>
    void Add(T elementToAdd);
}