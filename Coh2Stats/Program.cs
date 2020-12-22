using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			var games = MatchAnalysis.Build1v1MatchList(20, 10, 24);
			MatchAnalysis.ShowWinRates(games);
			Console.ReadLine();
		}
	}
}