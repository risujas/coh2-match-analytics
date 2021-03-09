using Newtonsoft.Json;

namespace Coh2Stats.RelicAPI
{
	public class PlayerIdentity
	{
		[JsonProperty("profile_id")] public int ProfileId { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("alias")] public string Alias { get; set; }
		[JsonProperty("personal_statgroup_id")] public int PersonalStatGroupId { get; set; }
		[JsonProperty("xp")] public int Xp { get; set; }
		[JsonProperty("level")] public int Level { get; set; }
		[JsonProperty("leaderboardregion_id")] public int LeaderboardRegionId { get; set; }
		[JsonProperty("country")] public string Country { get; set; }

		public int GetHighestRank(PlayerDatabase pdb, MatchTypeId matchTypeId)
		{
			var stat = pdb.GetHighestStatByStatGroup(PersonalStatGroupId, matchTypeId);

			if (stat == null)
			{
				return int.MaxValue;
			}

			if (stat.Rank < 1)
			{
				return int.MaxValue;
			}

			return stat.Rank;
		}
	}
}