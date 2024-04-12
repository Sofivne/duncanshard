namespace DuncanShard.Models;

/// <summary>
/// Class representing a wormhole. It is used to connect two shards. A user can travel from one shard to another using a wormhole.
/// </summary>
public class Wormhole
{
    public string BaseUri { get; set; }
    public string System { get; set; }
    public string SharedPassword { get; set; }
    public string User { get; set; }
    public string DestinationShard { get; set; }
    
}