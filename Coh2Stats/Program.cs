using System;

namespace Coh2Stats
{
	class Program
	{
        static void Main(string[] args)
		{
            var temp = AvailableLeaderboardsResponse.GetAvailableLeaderboards();
            for (int i = 0; i < temp.leaderboards.Count; i++)
			{
                Console.WriteLine(temp.leaderboards[i].name);
			}

            Console.ReadLine();
		}
	}
}