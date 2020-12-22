using System;

namespace Coh2Stats
{
	class Program
	{
		static void Main()
		{
			var games = MatchAnalysis.Build1v1MatchList(4, 50, 24*30);
			MatchAnalysis.ShowWinRates(games);
			Console.ReadLine();
		}
	}
}