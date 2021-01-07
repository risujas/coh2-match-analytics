using System;
using System.Collections.Generic;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			DatabaseBuilder db = new DatabaseBuilder();
			db.Build(RelicApi.GameModeId.OneVsOne);

			MatchAnalyticsBundle mab = MatchAnalyticsBundle.GetAllLoggedMatches();

			Console.ReadLine();
		}
	}
}