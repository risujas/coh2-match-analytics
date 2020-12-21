using System;

namespace Coh2Stats
{
	class Program
	{
        static void Main(string[] args)
		{
            var temp = PersonalStatResponse.GetPersonalStatBySteamId("76561198050674754");

			for (int i = 0; i < temp.leaderboardStats.Count; i++)
			{
				if (temp.leaderboardStats[i].rank != -1)
				{
					Console.WriteLine(temp.leaderboardStats[i].leaderboard_id + " " + temp.leaderboardStats[i].rank);
				}
			}

            Console.ReadLine();
		}
	}
}