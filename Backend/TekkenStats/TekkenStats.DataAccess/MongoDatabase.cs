using MongoDB.Driver;

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
}