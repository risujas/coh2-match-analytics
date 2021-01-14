using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Coh2Stats
{
	class JsonPlayerSummaries
	{
		public class Player
		{
			[JsonProperty("steamid")] public string SteamId { get; set; }
			[JsonProperty("communityvisibilitystate")] public int CommunityVisibilityState { get; set; }
			[JsonProperty("profilestate")] public int ProfileState { get; set; }
			[JsonProperty("personaname")] public string PersonaName { get; set; }
			[JsonProperty("commentpermission")] public int CommentPermission { get; set; }
			[JsonProperty("profileurl")] public string ProfileUrl { get; set; }
			[JsonProperty("avatar")] public string Avatar { get; set; }
			[JsonProperty("avatarmedium")] public string AvatarMedium { get; set; }
			[JsonProperty("avatarfull")] public string AvatarFull { get; set; }
			[JsonProperty("avatarhash")] public string AvatarHash { get; set; }
			[JsonProperty("personastate")] public int PersonaState { get; set; }
			[JsonProperty("primaryclanid")] public string PrimaryClanId { get; set; }
			[JsonProperty("timecreated")] public int TimeCreated { get; set; }
			[JsonProperty("personastateflags")] public int PersonaStateFlags { get; set; }
			[JsonProperty("loccountrycode")] public string LocCountryCode { get; set; }
		}

		public class Response
		{
			[JsonProperty("players")] public List<Player> Players { get; set; }
		}

		public class SteamResults
		{
			[JsonProperty("response")] public Response Response { get; set; }
		}

		public class Root
		{
			[JsonProperty("result")] public WebRequestResult Result { get; set; }
			[JsonProperty("avatars")] public List<PlayerIdentity> Avatars { get; set; }
			[JsonProperty("steamResults")] public SteamResults SteamResults { get; set; }
		}

		public static Root GetBySteamId(string steamId)
		{
			List<string> list = new List<string> { steamId };
			return GetBySteamId(list);
		}

		public static Root GetBySteamId(List<string> steamIds)
		{
			string idString = "\"" + string.Join("\",\"", steamIds) + "\"";
			string requestUrl = "https://coh2-api.reliclink.com/community/external/proxysteamuserrequest";
			string requestParams = "?request=/ISteamUser/GetPlayerSummaries/v0002/&title=coh2&profileNames=[" + idString + "]";

			var response = WebRequestHandler.GetStructuredJsonResponse<Root>(requestUrl, requestParams);

			if (response.Result.Message != "SUCCESS")
			{
				throw new Exception(response.Result.Message + ": " + idString);
			}

			return response;
		}
	}
}