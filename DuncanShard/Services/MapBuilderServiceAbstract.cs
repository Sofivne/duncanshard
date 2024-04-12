/**
 * @author LE GAL Florian
 * @date ${DATE}
 * @project ${PROJECT_NAME}
 */

using DuncanShard.Factories;
using DuncanShard.Models;
using Microsoft.Extensions.Options;
using Shard.Shared.Core;

namespace DuncanShard.Services;

public class MapBuilderServiceAbstract : IMapBuilderService
{
    public Sector Sector { get; } // TODO : Make this private
    public IList<Wormhole> Wormholes { get; }
    public Models.System GetRandomSystem()
    {
        return Sector.GetRandomSystem();
    }

    private const string ProjectName = "Test application";
    public MapBuilderServiceAbstract(IOptions<MapGeneratorOptions>options, BuildingFactory buildingFactory)
    {
        options.Value.Seed = ProjectName;
        var mapGen = new MapGenerator(options).Generate();
        Sector = new Sector(mapGen, buildingFactory);
        Wormholes = new List<Wormhole>();
    }
}