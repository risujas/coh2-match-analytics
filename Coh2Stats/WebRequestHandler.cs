using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Coh2Stats
{
	public class WebRequestResult
	{
		[JsonProperty("code")] public int Code { get; set; }
		[JsonProperty("message")] public string Message { get; set; }
	}

	internal class WebRequestHandler
	{
		private const long requestCooldownDuration = 1000;
		private static long requestCooldownStart = 0;

		public static string GetStringJsonResponse(string requestUrl, string requestParams)
		{
			string responseString;

			HttpClient client;
			using (client = new HttpClient())
			{
				client.BaseAddress = new Uri(requestUrl);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				while (true)
				{
					try
					{
						WaitForRequestCooldown();

						HttpResponseMessage responseObject = client.GetAsync(requestParams).Result;
						responseString = responseObject.Content.ReadAsStringAsync().Result;

						ResetRequestCooldown();

						break;
					}

					catch (Exception e)
					{
						UserIO.LogRootException(e);

						ResetRequestCooldown();
					}
				}

				return responseString;
			}

		}

		public static T GetStructuredJsonResponse<T>(string requestUrl, string requestParams)
		{
			string responseString = GetStringJsonResponse(requestUrl, requestParams);
			T structuredResponse = JsonConvert.DeserializeObject<T>(responseString);
			return structuredResponse;
		}

		private static void ResetRequestCooldown()
		{
			DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
			requestCooldownStart = dto.ToUnixTimeMilliseconds();
		}

		private static void WaitForRequestCooldown()
		{
			DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
			long current = dto.ToUnixTimeMilliseconds();

			long end = requestCooldownStart + requestCooldownDuration;
			long difference = end - current;

			if (difference > 0)
			{
				Thread.Sleep((int)difference);
			}
		}
	}
}