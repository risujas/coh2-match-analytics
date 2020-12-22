using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Coh2Stats
{
	class Program
	{
        static void Main(string[] args)
		{
			var temp = AvailableLeaderboardsResponse.Get();
			Console.WriteLine(temp.leaderboards[51].name);
			Console.ReadLine();
		}
	}
}