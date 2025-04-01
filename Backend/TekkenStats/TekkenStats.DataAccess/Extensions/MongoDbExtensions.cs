using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using TekkenStats.Core.Options;

namespace TekkenStats.DataAccess.Extensions;

public static class MongoDbExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var mongoOptions = configuration.GetRequiredSection(MongoOptions.Section).Get<MongoOptions>() ??
                           throw new Exception($"Missing {MongoOptions.Section}");

        var client = new MongoClient(mongoOptions.ConnectionString);
        services.AddSingleton(client);

        services.AddSingleton(new MongoDatabase(client, mongoOptions.DbName));
        return services;
    }
}

