using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MatchAnalysis
	{
		public static void ShowWinRates(List<RelicApi.RecentMatchHistory.MatchHistoryStat> uniqueMatches)
		{
			// factions
			// allies 0
			// axis 1

			// races
			// german 0
			// soviet 1
			// wgerman 2
			// aef 3
			// british 4

			int germanGames = 0;
			int germanWins = 0;
			int sovietGames = 0;
			int sovietWins = 0;
			int westGermanGames = 0;
			int westGermanWins = 0;
			int aefGames = 0;
			int aefWins = 0;
			int britishGames = 0;
			int britishWins = 0;

			foreach (var um in uniqueMatches)
			{
				foreach (var report in um.matchhistoryreportresults)
				{
					if (report.race_id == 0)
					{
						germanGames++;
						if (report.resulttype == 1)
						{
							germanWins++;
						}
					}

					if (report.race_id == 1)
					{
						sovietGames++;
						if (report.resulttype == 1)
						{
							sovietWins++;
						}
					}

					if (report.race_id == 2)
					{
						westGermanGames++;
						if (report.resulttype == 1)
						{
							westGermanWins++;
						}
					}

					if (report.race_id == 3)
					{
						aefGames++;
						if (report.resulttype == 1)
						{
							aefWins++;
						}
					}

					if (report.race_id == 4)
					{
						britishGames++;
						if (report.resulttype == 1)
						{
							britishWins++;
						}
					}
				}
			}

			Console.WriteLine("Wehrmacht win rate: {0}% ({1} out of {2} games)", ((float)germanWins / germanGames) * 100, germanWins, germanGames);
			Console.WriteLine("Soviet win rate: {0}% ({1} out of {2} games)", ((float)sovietWins / sovietGames) * 100, sovietWins, sovietGames);
			Console.WriteLine("Oberkommando West win rate: {0}% ({1} out of {2} games)", ((float)westGermanWins / westGermanGames) * 100, westGermanWins, westGermanGames);
			Console.WriteLine("United States Forces win rate: {0}% ({1} out of {2} games)", ((float)aefWins / aefGames) * 100, aefWins, aefGames);
			Console.WriteLine("British Forces win rate: {0}% ({1} out of {2} games)", ((float)britishWins / britishGames) * 100, britishWins, britishGames);
		}

		public static List<RelicApi.RecentMatchHistory.MatchHistoryStat> Build1v1MatchList(int startRank, int numRanks, int maxAgeHours)
		{
			List<RelicApi.RecentMatchHistory.MatchHistoryStat> uniqueMatches = new List<RelicApi.RecentMatchHistory.MatchHistoryStat>();

			var players = Build1v1PlayerList(startRank, numRanks);
			foreach (var p in players)
			{
				var response = RelicApi.RecentMatchHistory.GetByProfileId(p.profile_id.ToString());
				if (response.matchHistoryStats == null)
				{
					continue;
				}

				int numMatches = 0;
				int newMatches = 0;

				foreach (var m in response.matchHistoryStats)
				{
					if (m.maxplayers != 2)
					{
						continue;
					}

					DateTime cutoffTime = DateTime.Now.AddHours(-maxAgeHours);
					long cutoffUnixTime = ((DateTimeOffset)cutoffTime).ToUnixTimeSeconds();
					if (m.completiontime < cutoffUnixTime)
					{
						continue;
					}

					numMatches++;

					bool isUnique = true;
					foreach (var um in uniqueMatches)
					{
						if (um.id == m.id)
						{
							isUnique = false;
							break;
						}
					}

					if (isUnique)
					{
						newMatches++;
						uniqueMatches.Add(m);
					}
				}

				if (numMatches > 0)
				{
					Console.WriteLine("Found {0} matches ({1} new) for player {2} ({3})", numMatches, newMatches, p.name, p.alias);
				}
			}

			uniqueMatches = uniqueMatches.OrderBy(um => um.completiontime).ToList();
			return uniqueMatches;
		}

		private static List<RelicApi.Leaderboard.Member> Build1v1PlayerList(int startRank, int numRanks)
		{
			List<RelicApi.Leaderboard.Member> uniqueMembers = new List<RelicApi.Leaderboard.Member>();

			for (int i = 4; i <= 51; i++)
			{
				if (i > 7 && i < 51)
				{
					continue;
				}

				Console.WriteLine("Parsing leaderboard #{0}, ranks {1}-{2}", i, startRank, numRanks);

				var leaderboard = RelicApi.Leaderboard.GetById(i, startRank, numRanks);
				foreach (var stg in leaderboard.statGroups)
				{
					if (!uniqueMembers.Contains(stg.members[0]))
					{
						uniqueMembers.Add(stg.members[0]);
					}
				}

				Console.WriteLine("Total unique players: {0}", uniqueMembers.Count);
			}

			uniqueMembers = uniqueMembers.OrderBy(um => um.alias).ToList();
			return uniqueMembers;
		}
	}
}
