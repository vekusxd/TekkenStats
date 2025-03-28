using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TekkenStats.Core.Entities;

namespace TekkenStats.DataAccess;

public class MongoDatabase
{
    private readonly IMongoClient _mongoClient;
    private readonly string _dbName;

    public MongoDatabase(IMongoClient mongoClient, string dbName)
    {
        _mongoClient = mongoClient;
        _dbName = dbName;
    }
    
    public IMongoDatabase Db => _mongoClient.GetDatabase(_dbName);
    
    public  async Task InitIndexes()
    {
        var collection = Db.GetCollection<Player>(Player.CollectionName);

        var matchDateIndexDefinition = Builders<Player>.IndexKeys
            .Descending("Matches.Date");

        var idIndexModel = new CreateIndexModel<Player>(matchDateIndexDefinition);

        await collection.Indexes.CreateOneAsync(idIndexModel);
    }
}