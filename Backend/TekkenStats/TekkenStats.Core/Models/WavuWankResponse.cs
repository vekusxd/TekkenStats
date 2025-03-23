using System.Text;
using System.Text.Json.Serialization;

namespace TekkenStats.Core.Models;

public class WavuWankResponse
{
    [JsonPropertyName("battle_at")] public int BattleAt { get; set; }
    [JsonPropertyName("battle_id")] public required string BattleId { get; set; }
    [JsonPropertyName("battle_type")] public int BattleType { get; set; }
    [JsonPropertyName("game_version")] public int GameVersion { get; set; }
    [JsonPropertyName("p1_area_id")] public int? P1AreaId { get; set; }
    [JsonPropertyName("p1_chara_id")] public int P1CharaId { get; set; }
    [JsonPropertyName("p1_lang")] public string? P1Lang { get; set; }
    [JsonPropertyName("p1_name")] public required string P1Name { get; set; }
    [JsonPropertyName("p1_polaris_id")] public required string P1PolarisId { get; set; }
    [JsonPropertyName("p1_power")] public int P1Power { get; set; }
    [JsonPropertyName("p1_rank")] public int P1Rank { get; set; }
    [JsonPropertyName("p1_rating_before")] public int? P1RatingBefore { get; set; }
    [JsonPropertyName("p1_rating_change")] public int? P1RatingChange { get; set; }
    [JsonPropertyName("p1_region_id")] public int? P1RegionId { get; set; }
    [JsonPropertyName("p1_rounds")] public int P1Rounds { get; set; }
    [JsonPropertyName("p1_user_id")] public long P1UserId { get; set; }
    [JsonPropertyName("p2_area_id")] public int? P2AreaId { get; set; }
    [JsonPropertyName("p2_chara_id")] public int P2CharaId { get; set; }
    [JsonPropertyName("p2_lang")] public string? P2Lang { get; set; }
    [JsonPropertyName("p2_name")] public required string P2Name { get; set; }
    [JsonPropertyName("p2_polaris_id")] public required string P2PolarisId { get; set; }
    [JsonPropertyName("p2_power")] public int P2Power { get; set; }
    [JsonPropertyName("p2_rank")] public int P2Rank { get; set; }
    [JsonPropertyName("p2_rating_before")] public int? P2RatingBefore { get; set; }
    [JsonPropertyName("p2_rating_change")] public int? P2RatingChange { get; set; }
    [JsonPropertyName("p2_region_id")] public int? P2RegionId { get; set; }
    [JsonPropertyName("p2_rounds")] public int P2Rounds { get; set; }
    [JsonPropertyName("p2_user_id")] public long P2UserId { get; set; }
    [JsonPropertyName("stage_id")] public int StageId { get; set; }
    [JsonPropertyName("winner")] public int Winner { get; set; }
}