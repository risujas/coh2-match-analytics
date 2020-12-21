using System;

namespace Coh2Stats
{
	class Program
	{
        static void Main(string[] args)
		{
            var temp = RecentMatchHistoryResponse.GetRecentMatchHistoryBySteamId("76561198050674754");

			for (int i = 0; i < temp.profiles.Count; i++)
			{
                Console.WriteLine(temp.profiles[i].alias);
			}

            Console.ReadLine();
		}
	}
}