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
		private const long requestCooldownDurationMs = 1000;
		private static long requestCooldownStartMs = 0;

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
			requestCooldownStartMs = dto.ToUnixTimeMilliseconds();
		}

		private static void WaitForRequestCooldown()
		{
			DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
			long currentMs = dto.ToUnixTimeMilliseconds();

			long endMs = requestCooldownStartMs + requestCooldownDurationMs;
			long differenceMs = endMs - currentMs;

			if (differenceMs > 0)
			{
				Thread.Sleep((int)differenceMs);
			}
		}
	}
}