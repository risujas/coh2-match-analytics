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

			MatchAnalyticsBundle mab = MatchAnalyticsBundle.GetAllLoggedMatches().FilterByMap("crossroadswx");

			var games = mab.FilterByRace(RelicApi.RaceFlag.German).GetOrderedPlayCount();
			Console.WriteLine("\nOSTHEER");
			foreach (var g in games)
			{
				Console.WriteLine(g.Value + " " + g.Key);
			}

			games = mab.FilterByRace(RelicApi.RaceFlag.Soviet).GetOrderedPlayCount();
			Console.WriteLine("\nSOVIETS");
			foreach (var g in games)
			{
				Console.WriteLine(g.Value + " " + g.Key);
			}

			games = mab.FilterByRace(RelicApi.RaceFlag.WGerman).GetOrderedPlayCount();
			Console.WriteLine("\nOBERKOMMANDO");
			foreach (var g in games)
			{
				Console.WriteLine(g.Value + " " + g.Key);
			}

			games = mab.FilterByRace(RelicApi.RaceFlag.AEF).GetOrderedPlayCount();
			Console.WriteLine("\nUSF");
			foreach (var g in games)
			{
				Console.WriteLine(g.Value + " " + g.Key);
			}

			games = mab.FilterByRace(RelicApi.RaceFlag.British).GetOrderedPlayCount();
			Console.WriteLine("\nBRITISH");
			foreach (var g in games)
			{
				Console.WriteLine(g.Value + " " + g.Key);
			}

			Console.ReadLine();
		}
	}
}