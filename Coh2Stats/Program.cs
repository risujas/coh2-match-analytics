using System;

namespace Coh2Stats
{
	class Program
	{
        static void Main(string[] args)
		{
			var temp = PlayerSummariesResponse.GetPlayerSummariesBySteamId("76561198050674754");

			Console.WriteLine(temp.steamResults.response.players[0].avatarfull);

            Console.ReadLine();
		}
	}
}