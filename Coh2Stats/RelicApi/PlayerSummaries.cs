#pragma warning disable IDE1006
using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	namespace RelicApi
	{
		class PlayerSummaries
		{
			public class Result
			{
				public int code { get; set; }
				public string message { get; set; }
			}

			public class Avatar
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

			public class Player
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

			public class Response
			{
				public List<Player> players { get; set; }
			}

			public class SteamResults
			{
				public Response response { get; set; }
			}

			public class Root
			{
				public Result result { get; set; }
				public List<Avatar> avatars { get; set; }
				public SteamResults steamResults { get; set; }

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

				string requestUrl = "https://coh2-api.reliclink.com/community/external/proxysteamuserrequest";
				string requestParams = "?request=/ISteamUser/GetPlayerSummaries/v0002/&title=coh2&profileNames=[" + idString + "]";

				var response = WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

				if (response.result.message == "SUCCESS")
				{
					foreach (var x in response.avatars)
					{
						PlayerIdentity identity = new PlayerIdentity();
						identity.SteamId = x.name.ToString().Substring(x.name.ToString().LastIndexOf('/') + 1);
						identity.Nickname = x.alias;
						identity.ProfileId = x.profile_id;
						identity.PersonalStatGroupId = x.personal_statgroup_id;
						identity.Country = x.country;
						PlayerIdentityTracker.LogPlayer(identity);
					}
				}
				else
				{
					throw new Exception(response.result.message + ": " + idString);
				}

				return response;
			}
		}
	}
}
