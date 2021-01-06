#pragma warning disable IDE1006
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class PersonalStat
		{
			public class Result
			{
				public int code { get; set; }
				public string message { get; set; }
			}

			public class Member
			{
				public int profile_id { get; set; }
				public string name { get; set; }
				public string alias { get; set; }
				public int personal_statgroup_id { get; set; }
				public int xp { get; set; }
				public int level { get; set; }
				public int leaderboardregion_id { get; set; }
				public string country { get; set; }
			}

			public class StatGroup
			{
				public int id { get; set; }
				public string name { get; set; }
				public int type { get; set; }
				public List<Member> members { get; set; }
			}

			public class LeaderboardStat
			{
				public int statGroup_id { get; set; }
				public int leaderboard_id { get; set; }
				public int wins { get; set; }
				public int losses { get; set; }
				public int streak { get; set; }
				public int disputes { get; set; }
				public int drops { get; set; }
				public int rank { get; set; }
				public int rankTotal { get; set; }
				public int regionRank { get; set; }
				public int regionRankTotal { get; set; }
				public int rankLevel { get; set; }
				public int lastMatchDate { get; set; }
			}

			public class Root
			{
				public Result result { get; set; }
				public List<StatGroup> statGroups { get; set; }
				public List<LeaderboardStat> leaderboardStats { get; set; }

				private Root()
				{
				}
			}

			public static Root GetBySteamId(string steamId)
			{
				List<string> list = new List<string>();
				list.Add(steamId);
				return GetBySteamId(list);
			}

			public static Root GetBySteamId(List<string> steamIds)
			{
				string idString = "";
				for (int i = 0; i < steamIds.Count; i++)
				{
					idString += "\"/steam/";
					idString += steamIds[i];
					idString += "\"";

					if (steamIds.Count > i + 1)
					{
						idString += ", ";
					}
				}

				string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetPersonalStat";
				string requestParams = "?title=coh2&profile_names=[" + idString + "]";

				var response = WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

				foreach (var sg in response.statGroups)
				{
					foreach (var x in sg.members)
					{
						Profile profile = new Profile();
						profile.SteamId = x.name.Substring(x.name.LastIndexOf('/') + 1);
						profile.Nickname = x.alias;
						profile.ProfileId = x.profile_id.ToString();
						profile.PersonalStatGroupId = x.personal_statgroup_id.ToString();
						profile.Country = x.country;
						PlayerIdentityTracker.LogPlayer(profile);
					}
				}

				return response;
			}
		}
	}
}
