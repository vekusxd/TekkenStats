using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using TekkenStats.Application.Mappers;
using TekkenStats.Core.Entities;
using TekkenStats.DataAccess;

namespace TekkenStats.Application.Services;

public class WavuWankResponseProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<WavuWankResponseProcessor> _logger;
    private readonly HybridCache _cache;

    public WavuWankResponseProcessor(AppDbContext dbContext, ILogger<WavuWankResponseProcessor> logger,
        HybridCache cache)
    {
        _dbContext = dbContext;
        _logger = logger;
        _cache = cache;
    }

    //TODO привязать полученные сущности перед сохранением
    public async Task ProcessResponse(WavuWankResponse response)
    {
        var playersToAdd = new List<Player>();
        var playerNamesToAdd = new List<PlayerName>();
        var characterInfosToAdd = new List<CharacterInfo>();
        var battlesToAdd = new List<Battle>();

        try
        {
            await GetOrCreateBattle(response.BattleId, battlesToAdd);

            if (battlesToAdd.Count != 0)
            {
                return;
            }

            var player1Id = response.P1PolarisId;
            var player2Id = response.P2PolarisId;

            var player1 = await GetOrCreatePlayer(player1Id, playersToAdd);
            var player2 = await GetOrCreatePlayer(player2Id, playersToAdd);

            await TryAddName(player1, response.P1Name, playerNamesToAdd);
            await TryAddName(player2, response.P2Name, playerNamesToAdd);

            await _dbContext.SaveChangesAsync();

            var player1Char = await GetCharacter(response.P1CharaId);
            var player2Char = await GetCharacter(response.P2CharaId);

            var player1CharacterInfo =
                await GetOrCreateCharacterInfo(player1Id, response.P1CharaId, player1Char, characterInfosToAdd);
            var player2CharacterInfo =
                await GetOrCreateCharacterInfo(player2Id, response.P2CharaId, player1Char, characterInfosToAdd);


            if (response.Winner == 1)
            {
                player1CharacterInfo.WinCount++;
                player2CharacterInfo.LossCount++;
                player1.WinCount++;
                player2.LossCount++;
            }
            else
            {
                player1CharacterInfo.LossCount++;
                player2CharacterInfo.WinCount++;
                player1.LossCount++;
                player2.WinCount++;
            }

            player1CharacterInfo.Rating = response.P1Rank;
            player2CharacterInfo.Rating = response.P2Rank;

            var battle = new Battle
            {
                Id = response.BattleId,
                GameVersion = response.GameVersion.ToString(),
                PlayedDateTime = DateTimeOffset.FromUnixTimeSeconds(response.BattleAt).UtcDateTime,
                Player1Id = player1Id,
                Player2Id = player2Id,
                PlayerCharacter1Id = response.P1CharaId,
                PlayerCharacter2Id = response.P2CharaId,
                Player1 = player1,
                Player2 = player2,
                PlayerCharacter1 = player1Char,
                PlayerCharacter2 = player2Char,
                Player1RoundsCount = response.P1Rounds,
                Player2RoundsCount = response.P2Rounds,
                Player1RatingBefore = response.P1RatingBefore ?? 0,
                Player2RatingBefore = response.P2RatingBefore ?? 0,
                Player1RatingChange = response.P1RatingChange ?? 0,
                Player2RatingChange = response.P2RatingChange ?? 0,
                Winner = response.Winner
            };

            battlesToAdd.Add(battle);

            await _dbContext.AddRangeAsync(playersToAdd);
            await _dbContext.AddRangeAsync(playerNamesToAdd);
            await _dbContext.AddRangeAsync(characterInfosToAdd);
            await _dbContext.AddRangeAsync(battlesToAdd);
            
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occured: {ErrorMessage}", ex.Message);
        }
    }

    private async Task<Character> GetCharacter(int characterId)
    {
        var character = await _cache.GetOrCreateAsync($"character:{characterId}", async (token) =>
        {
            var innerCharacter = await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == characterId,
                                     cancellationToken: token) ??
                                 throw new NullReferenceException($"Character with id: {characterId} not found");
            return CharacterMapper.FromEntity(innerCharacter);
        });
        return CharacterMapper.ToEntity(character);
    }

    private async Task TryAddName(Player player, string username, List<PlayerName> playerNamesToAdd)
    {
        await _dbContext.Entry(player).Collection(p => p.Names).LoadAsync();
        var contains = player.Names.Any(n => n.Name == username);
        if (contains) return;

        var playerName = new PlayerName
        {
            Name = username,
            NormalizedName = username.ToUpperInvariant(),
            Date = DateOnly.FromDateTime(DateTime.UtcNow),
            PlayerId = player.Id
        };
        playerNamesToAdd.Add(playerName);
    }

    private async Task<CharacterInfo> GetOrCreateCharacterInfo(string playerId, int characterId,
        Character character,
        List<CharacterInfo> characterInfosToAdd)
    {
        var characterInfo = await _dbContext.CharacterInfos
            .FirstOrDefaultAsync(ci => ci.PlayerId == playerId && ci.CharacterId == characterId);

        if (characterInfo != null) return characterInfo;

        characterInfo = new CharacterInfo
        {
            CharacterId = characterId,
            Character = character,
            PlayerId = playerId,
            WinCount = 0,
            LossCount = 0,
            LastPlayed = DateOnly.FromDateTime(DateTime.UtcNow),
            Rating = 0
        };
        characterInfosToAdd.Add(characterInfo);
        return characterInfo;
    }

    private async Task<Battle> GetOrCreateBattle(string battleId, List<Battle> battlesToAdd)
    {
        var battle = await _cache.GetOrCreateAsync($"battle:{battleId}", async (token) =>
            {
                var battle =
                    await _dbContext.Battles.FirstOrDefaultAsync(b => b.Id == battleId, cancellationToken: token);

                if (battle != null) return BattleMapper.FromEntity(battle);

                battle = new Battle
                {
                    Id = battleId,
                    GameVersion = string.Empty,
                    Player1Id = string.Empty,
                    Player2Id = string.Empty
                };

                battlesToAdd.Add(battle);

                return BattleMapper.FromEntity(battle);
            },
            new HybridCacheEntryOptions
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(2),
                Expiration = TimeSpan.FromMinutes(2)
            });
        return BattleMapper.ToEntity(battle);
    }

    private async Task<Player> GetOrCreatePlayer(string playerId, List<Player> playersToAdd)
    {
        var existingPlayer = playersToAdd.FirstOrDefault(p => p.Id == playerId);
        if (existingPlayer != null)
        {
            return existingPlayer;
        }
        var player = await _cache.GetOrCreateAsync(
            key: $"player:{playerId}",
            async (token) =>
            {
                var dbPlayer = await _dbContext.Players
                    .FirstOrDefaultAsync(p => p.Id == playerId, cancellationToken: token);
                if (dbPlayer != null)
                {
                    return PlayerMapper.FromEntity(dbPlayer);
                }

                var newPlayer = new Player
                {
                    Id = playerId,
                    WinCount = 0,
                    LossCount = 0
                };

                playersToAdd.Add(newPlayer);

                return PlayerMapper.FromEntity(newPlayer);
            }
        );
    
        var cachedPlayer = PlayerMapper.ToEntity(player);
        return playersToAdd.FirstOrDefault(p => p.Id == playerId) ?? cachedPlayer;
    }
}