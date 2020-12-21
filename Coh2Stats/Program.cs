using System;

namespace Coh2Stats
{
	class Program
	{
        static void Main(string[] args)
		{
            var lb = LeaderboardResponse.GetLeaderboardById(5, 1, 10);
            for (int i = 0; i < lb.statGroups.Count; i++)
			{
                Console.WriteLine(lb.statGroups[i].members[0].alias + " " + lb.statGroups[i].members[0].country + " " + lb.statGroups[i].members[0].profile_id);
			}

            Console.ReadLine();
		}
	}
}