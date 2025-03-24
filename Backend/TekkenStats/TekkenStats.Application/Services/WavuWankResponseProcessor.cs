using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TekkenStats.Core.Models;
using TekkenStats.DataAccess;

namespace TekkenStats.Application.Services;

public class WavuWankResponseProcessor
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<WavuWankResponseProcessor> _logger;

    public WavuWankResponseProcessor(AppDbContext dbContext, ILogger<WavuWankResponseProcessor> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task ProcessResponse(WavuWankResponse response)
    {
        var playersToAdd = new List<Player>();
        var playerNamesToAdd = new List<PlayerName>();
        var characterInfosToAdd = new List<CharacterInfo>();
        var battlesToAdd = new List<Battle>();

        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            var existsBattle = await _dbContext.Battles.FirstOrDefaultAsync(b => b.Id == response.BattleId);
            if (existsBattle != null)
            {
                await transaction.RollbackAsync();
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

            await _dbContext.SaveChangesAsync();

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

            await _dbContext.BulkInsertOrUpdateAsync(playersToAdd);
            await _dbContext.BulkInsertOrUpdateAsync(playerNamesToAdd);
            await _dbContext.BulkInsertOrUpdateAsync(characterInfosToAdd);
            await _dbContext.BulkInsertOrUpdateAsync(battlesToAdd);

            await transaction.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occured: {ErrorMessage}", ex.Message);
            await transaction.RollbackAsync();
        }
    }

    private async Task<Character> GetCharacter(int characterId)
    {
        return await _dbContext.Characters.FirstOrDefaultAsync(c => c.Id == characterId) ??
               throw new Exception($"Character with id: {characterId} not found");
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

    private async Task<CharacterInfo> GetOrCreateCharacterInfo(string playerId, int characterId, Character character,
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

    private async Task<Player> GetOrCreatePlayer(string playerId, List<Player> playersToAdd)
    {
        var player = await PlayerExists(playerId);
        if (player != null) return player;

        player = new Player
        {
            Id = playerId,
            WinCount = 0,
            LossCount = 0
        };
        playersToAdd.Add(player);
        return player;
    }

    private async Task<Player?> PlayerExists(string playerId)
    {
        return await _dbContext.Players
            .FirstOrDefaultAsync(p => p.Id == playerId);
    }
}