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
		public static string GetStringJsonResponse(string requestUrl, string requestParams)
		{
			string responseString = "";

			HttpClient client;
			using (client = new HttpClient())
			{
				client.BaseAddress = new Uri(requestUrl);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				int maxAttempts = 10;
				int attemptInterval = 1000;

				for (int i = 1; i <= maxAttempts; i++)
				{
					try
					{
						HttpResponseMessage responseObject = client.GetAsync(requestParams).Result;
						responseString = responseObject.Content.ReadAsStringAsync().Result;
						break;
					}

					catch (Exception e)
					{
						Console.Write(e.Message);

						Thread.Sleep(attemptInterval);

						if (i == maxAttempts)
						{
							throw e;
						}
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
	}
}