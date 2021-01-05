using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Coh2Stats
{
	class Utilities
	{
		public static string GetStringJsonResponse(string requestUrl, string requestParams)
		{
			string responseString;

			HttpClient client;
			using (client = new HttpClient())
			{
				client.BaseAddress = new Uri(requestUrl);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				HttpResponseMessage responseObject = client.GetAsync(requestParams).Result;
				if (responseObject.IsSuccessStatusCode)
				{
					responseString = responseObject.Content.ReadAsStringAsync().Result;

				}
				else
				{
					throw new Exception((int)responseObject.StatusCode + " (" + responseObject.ReasonPhrase + ")");
				}
			}
			
			return responseString;
		}

		public static T GetStructuredJsonResponse<T>(string requestUrl, string requestParams)
		{
			string responseString = GetStringJsonResponse(requestUrl, requestParams);
			T structuredResponse = JsonConvert.DeserializeObject<T>(responseString);
			return structuredResponse;
		}

		public static string GetSteamIdByProfileId(string profileId)
		{
			string steamId = "";

			var personalStat = RelicApi.PersonalStat.GetByProfileId(profileId);
			foreach (var sg in personalStat.statGroups)
			{
				if (sg.type == 1)
				{
					steamId = sg.members[0].name;
					steamId = steamId.Substring(steamId.LastIndexOf('/') + 1, 17);

					break;
				}
			}

			return steamId;
		}

		public static string GetProfileIdBySteamId(string steamId)
		{
			string profileId = "";

			var personalStat = RelicApi.PersonalStat.GetBySteamId(steamId);
			foreach (var sg in personalStat.statGroups)
			{
				if (sg.type == 1)
				{
					profileId = sg.members[0].profile_id.ToString();

					break;
				}
			}

			return profileId;
		}

		public static string GetAliasByProfileId(string profileId)
		{
			string alias = "";

			var personalStat = RelicApi.PersonalStat.GetByProfileId(profileId);
			foreach (var sg in personalStat.statGroups)
			{
				if (sg.type == 1)
				{
					alias = sg.members[0].alias;
					break;
				}
			}

			return alias;
		}

		public static string GetAliasBySteamId(string steamId)
		{
			string alias = "";

			var personalStat = RelicApi.PersonalStat.GetBySteamId(steamId);
			foreach (var sg in personalStat.statGroups)
			{
				if (sg.type == 1)
				{
					alias = sg.members[0].alias;
					break;
				}
			}

			return alias;
		}

		public static string GetHumanReadableRaceId(int raceId)
		{
			string name = "";

			if (raceId == 0)
			{
				name = "OST";
			}

			if (raceId == 1)
			{
				name = "SOV";
			}

			if (raceId == 2)
			{
				name = "OKW";
			}

			if (raceId == 3)
			{
				name = "USF";
			}

			if (raceId == 4)
			{
				name = "UKF";
			}

			return name;
		}

		public static string GetHumanReadableTeamId(int teamId)
		{
			string name = "";

			if (teamId == 0)
			{
				name = "Axis";
			}

			if (teamId == 1)
			{
				name = "Allies";
			}

			return name;
		}

		public static string GetHumanReadableResultType(int resultType)
		{
			string name = "";

			if (resultType == 0)
			{
				name = "Defeat";
			}

			if (resultType == 1)
			{
				name = "Victory";
			}

			return name;
		}
	}
}