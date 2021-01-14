using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats
{
	class JsonPersonalStat
	{
		public class Root
		{
			[JsonProperty("result")] public WebRequestResult Result { get; set; }
			[JsonProperty("statGroups")] public List<StatGroup> StatGroups { get; set; }
			[JsonProperty("leaderboardStats")] public List<LeaderboardStat> LeaderboardStats { get; set; }
		}

		public static Root GetByProfileId(int profileId)
		{
			List<int> list = new List<int> { profileId };
			return GetByProfileId(list);
		}

		public static Root GetByProfileId(List<int> profileIds)
		{
			string idString = string.Join(",", profileIds);
			string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetPersonalStat";
			string requestParams = "?title=coh2&profile_ids=[" + idString + "]";

			return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
		}
	}
}