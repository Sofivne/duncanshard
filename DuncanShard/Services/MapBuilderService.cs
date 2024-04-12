using DuncanShard.Factories;
using Microsoft.Extensions.Options;
using Shard.Shared.Core;

namespace DuncanShard.Services;

public class MapBuilderService : MapBuilderServiceAbstract
{
    // TODO : remove this class
    public MapBuilderService(IOptions<MapGeneratorOptions> options, BuildingFactory buildingFactory) : base(options, buildingFactory)
    {
    }
}