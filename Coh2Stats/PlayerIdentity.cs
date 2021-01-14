using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coh2Stats
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

		public int GetHighestRank(Database db)
		{
			var stat = db.GetHighestStatByStatGroup(PersonalStatGroupId);
			if (stat == null)
			{
				return int.MaxValue;
			}

			return stat.Rank;
		}

		public void PrintPlayer()
		{
			Console.WriteLine(ProfileId + " " + Name + " " + PersonalStatGroupId + " " + Xp + " " + Level + " " + Country + " " + LeaderboardRegionId + " " + Alias);
		}
	}
}
