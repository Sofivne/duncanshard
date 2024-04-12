namespace DuncanShard.Configuration;

public class DatabaseSettings
{
    public MongoDBConfig MongoDB { get; set; } = new MongoDBConfig();

}