using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		// 76561198050674754
		static void Main()
		{
			List<string> list = new List<string>();
			list.Add("76561198094150208");
			list.Add("76561198050674754");
			var response = RelicApi.RecentMatchHistory.GetBySteamId(list);

			int mp2 = 0;
			int mp4 = 0;
			int mp6 = 0;
			int mp8 = 0;
			foreach (var mhs in response.matchHistoryStats)
			{
				if (mhs.description != "AUTOMATCH")
				{
					continue;
				}

				if (mhs.maxplayers == 2)
				{
					mp2++;
				}

				if (mhs.maxplayers == 4)
				{
					mp4++;
				}

				if (mhs.maxplayers == 6)
				{
					mp6++;
				}

				if (mhs.maxplayers == 8)
				{
					mp8++;
				}
			}

			PlayerIdentityTracker.PrintLoggedPlayers();
			Console.WriteLine(mp2 + " " + mp4 + " " + mp6 + " " + mp8);

			Console.ReadLine();
		}
	}
}