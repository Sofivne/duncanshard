using DuncanShard.Factories;
using DuncanShard.Models;
using Microsoft.Extensions.Options;
using Shard.Shared.Core;

namespace DuncanShard.Services;

public class MapWithWormholesBuilderService : MapBuilderServiceAbstract
{
    public MapWithWormholesBuilderService(IOptions<MapGeneratorOptions> options, IOptions<Dictionary<string, Wormhole>> wormholes, BuildingFactory buildingFactory) : base(options, buildingFactory)
    {
        wormholes.Value.ToList().ForEach(kv =>
        {
            var wormhole = kv.Value;
            var system = Sector.GetSystemByName(wormhole.System);
            if (system is null)
                return;
            wormhole.DestinationShard = kv.Key;
            Wormholes.Add(wormhole);
        });
    }
}