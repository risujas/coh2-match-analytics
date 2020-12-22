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

			Console.WriteLine("Total games: {0}", (germanGames + sovietGames + westGermanGames + aefGames + britishGames) / 2);
			Console.WriteLine("Wehrmacht win rate: {0:0.0}% ({1} out of {2} games)", ((float)germanWins / germanGames) * 100, germanWins, germanGames);
			Console.WriteLine("Soviet win rate: {0:0.0}% ({1} out of {2} games)", ((float)sovietWins / sovietGames) * 100, sovietWins, sovietGames);
			Console.WriteLine("Oberkommando West win rate: {0:0.0}% ({1} out of {2} games)", ((float)westGermanWins / westGermanGames) * 100, westGermanWins, westGermanGames);
			Console.WriteLine("United States Forces win rate: {0:0.0}% ({1} out of {2} games)", ((float)aefWins / aefGames) * 100, aefWins, aefGames);
			Console.WriteLine("British Forces win rate: {0:0.0}% ({1} out of {2} games)", ((float)britishWins / britishGames) * 100, britishWins, britishGames);
		}

		public static List<RelicApi.RecentMatchHistory.MatchHistoryStat> Build1v1MatchList(int level, int numPlayers, int maxAgeHours)
		{
			List<RelicApi.RecentMatchHistory.MatchHistoryStat> uniqueMatches = new List<RelicApi.RecentMatchHistory.MatchHistoryStat>();

			var players = Build1v1PlayerList(level, numPlayers);
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
					if (m.startgametime < cutoffUnixTime)
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
			}

			uniqueMatches = uniqueMatches.OrderBy(um => um.startgametime).ToList();
			return uniqueMatches;
		}

		private static List<RelicApi.Leaderboard.Member> Build1v1PlayerList(int level, int numPlayers)
		{
			List<RelicApi.Leaderboard.Member> uniqueMembers = new List<RelicApi.Leaderboard.Member>();

			for (int i = 4; i <= 51; i++)
			{
				if (i > 7 && i < 51)
				{
					continue;
				}

				int equivalentRank = RelicApi.Leaderboard.FindEquivalentRankForLevel(i, level);
				var leaderboard = RelicApi.Leaderboard.GetById(i, equivalentRank, numPlayers);

				foreach (var stg in leaderboard.statGroups)
				{
					if (!uniqueMembers.Contains(stg.members[0]))
					{
						uniqueMembers.Add(stg.members[0]);
					}
				}
			}

			uniqueMembers = uniqueMembers.OrderBy(um => um.alias).ToList();
			return uniqueMembers;
		}
	}
}
