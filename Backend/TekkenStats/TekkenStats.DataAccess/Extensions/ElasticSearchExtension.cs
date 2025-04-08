using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TekkenStats.Core.Options;

namespace TekkenStats.DataAccess.Extensions;

public static class ElasticSearchExtension
{
    public static IServiceCollection AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
    {
        var elasticOptions = configuration.GetRequiredSection(ElasticOptions.Section).Get<ElasticOptions>() ??
                             throw new Exception($"Missing {ElasticOptions.Section}");

        var settings = new ElasticsearchClientSettings(new Uri(elasticOptions.Url));
        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);
        services.AddSingleton<ElasticSearch>();

        return services;
    }
}