#pragma warning disable IDE1006
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class PlayerSummaries
		{
			public struct Result
			{
				public int code { get; set; }
				public string message { get; set; }
			}

			public struct Avatar
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

			public struct Player
			{
				public string steamid { get; set; }
				public int communityvisibilitystate { get; set; }
				public int profilestate { get; set; }
				public string personaname { get; set; }
				public int commentpermission { get; set; }
				public string profileurl { get; set; }
				public string avatar { get; set; }
				public string avatarmedium { get; set; }
				public string avatarfull { get; set; }
				public string avatarhash { get; set; }
				public int personastate { get; set; }
				public string primaryclanid { get; set; }
				public int timecreated { get; set; }
				public int personastateflags { get; set; }
				public string loccountrycode { get; set; }
			}

			public struct Response
			{
				public List<Player> players { get; set; }
			}

			public struct SteamResults
			{
				public Response response { get; set; }
			}

			public struct Root
			{
				public Result result { get; set; }
				public List<Avatar> avatars { get; set; }
				public SteamResults steamResults { get; set; }
			}

			public static Root GetBySteamId(string steamId)
			{
				string requestUrl = "https://coh2-api.reliclink.com/community/external/proxysteamuserrequest";
				string requestParams = "?request=/ISteamUser/GetPlayerSummaries/v0002/&title=coh2&profileNames=[\"/steam/" + steamId + "\"]";

				return Utilities.GetStructuredJsonResponse<Root>(requestUrl, requestParams);
			}
		}
	}
}
