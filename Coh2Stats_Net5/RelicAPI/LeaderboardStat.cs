using Newtonsoft.Json;

namespace Coh2Stats_Net5.RelicAPI
{
	public class LeaderboardStat
	{
		[JsonProperty("statGroup_id")] public int StatGroupId { get; set; }
		[JsonProperty("leaderboard_id")] public int LeaderboardId { get; set; }
		[JsonProperty("wins")] public int Wins { get; set; }
		[JsonProperty("losses")] public int Losses { get; set; }
		[JsonProperty("streak")] public int Streak { get; set; }
		[JsonProperty("disputes")] public int Disputes { get; set; }
		[JsonProperty("drops")] public int Drops { get; set; }
		[JsonProperty("rank")] public int Rank { get; set; }
		[JsonProperty("rankTotal")] public int RankTotal { get; set; }
		[JsonProperty("regionRank")] public int RegionRank { get; set; }
		[JsonProperty("regionRankTotal")] public int RegionRankTotal { get; set; }
		[JsonProperty("rankLevel")] public int RankLevel { get; set; }
		[JsonProperty("lastMatchDate")] public int LastMatchDate { get; set; }
	}
}
