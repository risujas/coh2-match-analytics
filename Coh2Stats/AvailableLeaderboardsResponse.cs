using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats
{
	class AvailableLeaderboardsResponse
	{
        public class Result
        {
            public int code { get; set; }
            public string message { get; set; }
        }

        public class Leaderboardmap
        {
            public int matchtype_id { get; set; }
            public int statgroup_type { get; set; }
            public int race_id { get; set; }
        }

        public class Leaderboard
        {
            public int id { get; set; }
            public string name { get; set; }
            public int isranked { get; set; }
            public List<Leaderboardmap> leaderboardmap { get; set; }
        }

        public class MatchType
        {
            public int id { get; set; }
            public string name { get; set; }
            public int locstringid { get; set; }
            public string localizedName { get; set; }
        }

        public class Race
        {
            public int id { get; set; }
            public string name { get; set; }
            public int faction_id { get; set; }
            public int locstringid { get; set; }
            public string localizedName { get; set; }
        }

        public class Faction
        {
            public int id { get; set; }
            public string name { get; set; }
            public int locstringid { get; set; }
            public string localizedName { get; set; }
        }

        public class LeaderboardRegion
        {
            public int id { get; set; }
            public string name { get; set; }
            public int locstringid { get; set; }
        }

        public class Root
        {
            public Result result { get; set; }
            public List<Leaderboard> leaderboards { get; set; }
            public List<MatchType> matchTypes { get; set; }
            public List<Race> races { get; set; }
            public List<Faction> factions { get; set; }
            public List<LeaderboardRegion> leaderboardRegions { get; set; }
        }

        public static Root GetAvailableLeaderboards()
        {
            string requestUrl = "https://coh2-api.reliclink.com/community/leaderboard/GetAvailableLeaderboards";
            string requestParams = "?title=coh2";

            return WebUtils.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
        }
    }
}
