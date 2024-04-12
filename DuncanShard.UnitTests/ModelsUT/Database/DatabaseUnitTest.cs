using System.Reflection;
using DuncanShard.Configuration;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using Xunit;

namespace DuncanShard.UnitTests.ModelsUT.Database
{
    public class DatabaseUnitTest
    {
        private readonly MongoDBConfig _config;

        public DatabaseUnitTest()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            _config = new MongoDBConfig();
            configuration.GetSection("MongoDBConfig").Bind(_config);
        }

        [Fact]
        public void ConnectionString_WithAuthentication_ReturnsCorrectFormat()
        {
            var connectionString = _config.ConnectionString;
            Assert.Equal($"mongodb://{_config.Host}:{_config.Port}", connectionString);
        }

        

        [Fact]
        public void ConnectionWithHostEmpty()
        {
            var config = new MongoDBConfig
            {
                Host = "",
                Port = 27017,
                User = "",
                Password = "",
            };
            Assert.Throws<Exception>(() => config.ConnectionString);
        }

        [Fact]
        public void ConnectionWithPortInvalid()
        {
            var config = new MongoDBConfig
            {
                Host = "localhost",
                Port = 0,
                User = "",
                Password = "",
            };
            Assert.Throws<Exception>(() => config.ConnectionString);
        }
        
        
        [Fact]
        public void CanConnectToDatabase_ReturnsTrue()
        {
            // Arrange
            var connectionString = _config.ConnectionString;
            var client = new MongoClient(connectionString);

            // Act
            var database = client.GetDatabase(_config.Database);
            var command = new BsonDocument { { "ping", 1 } };
            var result = database.RunCommand<BsonDocument>(command);

            // Assert
            Assert.Equal(1, result["ok"].AsDouble);
        }
    }
}
