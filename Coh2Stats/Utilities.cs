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

		public static string GetSteamIdFromProfileId(string profileId)
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

		public static string GetProfileIdFromSteamId(string steamId)
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
	}
}
