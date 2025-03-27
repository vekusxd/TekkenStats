using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TekkenStats.Core.Entities;
using TekkenStats.Core.Options;

namespace TekkenStats.DataAccess.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var client = new MongoClient(configuration["MongoOptions:ConnectionString"]);
        services.AddSingleton(client);

        var mongoOptions = configuration.GetRequiredSection(MongoOptions.Section).Get<MongoOptions>() ??
                           throw new Exception($"Missing {MongoOptions.Section}");

        services.AddSingleton(new MongoDatabase(client, mongoOptions.DbName));
        return services;
    }

    public static async Task InitIndexes(this IServiceProvider serviceProvider)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<MongoDatabase>().Db;
        var collection = db.GetCollection<Player>(Player.CollectionName);

        var matchDateIndexDefinition = Builders<Player>.IndexKeys
            .Descending("Matches.Date");

        var idIndexModel = new CreateIndexModel<Player>(matchDateIndexDefinition);

        await collection.Indexes.CreateOneAsync(idIndexModel);
    }
}