using System;

namespace Coh2Stats
{
	class Program
	{
        static void Main(string[] args)
		{
            var temp = RecentMatchHistoryResponse.GetRecentMatchHistoryByProfileId("183282");

			for (int i = 0; i < temp.profiles.Count; i++)
			{
                Console.WriteLine(temp.profiles[i].alias);
			}

            Console.ReadLine();
		}
	}
}