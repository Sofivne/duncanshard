using DuncanShard.Models;

namespace DuncanShard.Services;

public interface IMapBuilderService
{
    public Sector Sector { get; }
    public IList<Wormhole> Wormholes { get; }
    public Models.System GetRandomSystem();
    
}