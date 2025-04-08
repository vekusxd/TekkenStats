using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Logging;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess.Models;

namespace TekkenStats.DataAccess;

public class ElasticSearch
{
    private readonly ElasticsearchClient _client;
    private readonly ILogger<ElasticSearch> _logger;

    public ElasticsearchClient Client => _client;

    public ElasticSearch(ElasticsearchClient client, ILogger<ElasticSearch> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task InitIndexes()
    {
        await _client.Indices.CreateAsync<IndexedPlayer>(IndexedPlayer.IndexName);
    }

    public async Task IndexPlayer(string tekkenId, string name)
    {
        var id = $"{tekkenId}:{name}";

        var indexedPlayer = new IndexedPlayer
        {
            Name = name,
            TekkenId = tekkenId,
        };

        var upsertResult = await _client.UpdateAsync<IndexedPlayer, IndexedPlayer>(IndexedPlayer.IndexName, id,
            configure =>
            {
                configure.Upsert(indexedPlayer);
                configure.Doc(indexedPlayer);
            });


        if (!upsertResult.IsValidResponse)
        {
            _logger.LogError("Error while indexing player: {}", upsertResult.DebugInformation);
        }
    }
}