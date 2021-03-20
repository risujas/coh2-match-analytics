using System;
using System.Diagnostics;
using System.Threading;

namespace Coh2Stats
{
	public class Program
	{
		public static void HandleGameMode(MatchTypeId gameMode)
		{
			DatabaseHandler.Load(gameMode);
			DatabaseHandler.ParseAndProcess(gameMode);
		}

		private static void Main()
		{
			UserIO.PrintStartingInfo();

			while (true)
			{
				HandleGameMode(MatchTypeId._1v1_);
				HandleGameMode(MatchTypeId._2v2_);
				HandleGameMode(MatchTypeId._3v3_);
				HandleGameMode(MatchTypeId._4v4_);
			}
		}
	}
}