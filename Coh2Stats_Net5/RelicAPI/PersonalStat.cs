using Newtonsoft.Json;

using System.Collections.Generic;

namespace Coh2Stats.RelicAPI
{
    public class PersonalStat
    {
        public class Root
        {
            [JsonProperty("result")] public WebRequestResult Result { get; set; }
            [JsonProperty("statGroups")] public List<StatGroup> StatGroups { get; set; }
            [JsonProperty("leaderboardStats")] public List<LeaderboardStat> LeaderboardStats { get; set; }
        }

        public static Root RequestByProfileId(List<int> profileIds)
        {
            string idString;

            if (profileIds.Count > 1)
            {
                idString = string.Join(",", profileIds);
            }
            else
            {
                idString = profileIds[0].ToString();
            }

            string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetPersonalStat";
            string requestParams = "?title=coh2&profile_ids=[" + idString + "]";

            return WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
        }
    }
}
