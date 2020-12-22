using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MatchAnalysis
	{
		public enum Race
		{
			German = 0,
			Soviet = 1,
			WestGerman = 2,
			AEF = 3,
			British = 4
		}

		public struct Result
		{
			public Race alliedFactionPick;
			public Race axisFactionPick;
			public bool axisVictory;
		}

		public class ResultBundle
		{
			public List<Result> matchResults = new List<Result>();

			public int NumGermanMatches { get; private set; } = 0;
			public int NumGermanWins { get; private set; } = 0;
			public int NumSovietMatches { get; private set; } = 0;
			public int NumSovietWins { get; private set; } = 0;
			public int NumWestGermanMatches { get; private set; } = 0;
			public int NumWestGermanWins { get; private set; } = 0;
			public int NumAefMatches { get; private set; } = 0;
			public int NumAefWins { get; private set; } = 0;
			public int NumBritishMatches { get; private set; } = 0;
			public int NumBritishWins { get; private set; } = 0;

			public void ParseMatches(List<RelicApi.RecentMatchHistory.MatchHistoryStat> uniqueMatches)
			{
				foreach (var um in uniqueMatches)
				{
					Result res = new Result();

					foreach (var report in um.matchhistoryreportresults)
					{
						if (report.race_id == (int)Race.German)
						{
							res.axisFactionPick = (Race)report.race_id;
							NumGermanMatches++;

							if (report.resulttype == 1)
							{
								res.axisVictory = true;
								NumGermanWins++;
							}
						}

						if (report.race_id == (int)Race.Soviet)
						{
							res.alliedFactionPick = (Race)report.race_id;
							NumSovietMatches++;

							if (report.resulttype == 1)
							{
								res.axisVictory = false;
								NumSovietWins++;
							}
						}

						if (report.race_id == (int)Race.WestGerman)
						{
							res.axisFactionPick = (Race)report.race_id;
							NumWestGermanMatches++;

							if (report.resulttype == 1)
							{
								res.axisVictory = true;
								NumWestGermanWins++;
							}
						}

						if (report.race_id == (int)Race.AEF)
						{
							res.alliedFactionPick = (Race)report.race_id;
							NumAefMatches++;

							if (report.resulttype == 1)
							{
								res.axisVictory = false;
								NumAefWins++;
							}
						}

						if (report.race_id == (int)Race.British)
						{
							res.alliedFactionPick = (Race)report.race_id;
							NumBritishMatches++;

							if (report.resulttype == 1)
							{
								res.axisVictory = false;
								NumBritishWins++;
							}
						}
					}

					matchResults.Add(res);
				}
			}

			public void PrintWinRates()
			{
				Console.WriteLine("Soviet: {0:0.0}% ({1} out of {2})", ((float)NumSovietWins / NumSovietMatches) * 100, NumSovietWins, NumSovietMatches);
				Console.WriteLine("Wehrmacht: {0:0.0}% ({1} out of {2})", ((float)NumGermanWins / NumGermanMatches) * 100, NumGermanWins, NumGermanMatches);
				Console.WriteLine("Oberkommando West: {0:0.0}% ({1} out of {2})", ((float)NumWestGermanWins / NumWestGermanMatches) * 100, NumWestGermanWins, NumWestGermanMatches);
				Console.WriteLine("United States Forces: {0:0.0}% ({1} out of {2})", ((float)NumAefWins / NumAefMatches) * 100, NumAefWins, NumAefMatches);
				Console.WriteLine("British Forces: {0:0.0}% ({1} out of {2})", ((float)NumBritishWins / NumBritishMatches) * 100, NumBritishWins, NumBritishMatches);
			}
		}

		public static List<RelicApi.RecentMatchHistory.MatchHistoryStat> Get1v1MatchList(int level, int numPlayers, int maxAgeHours)
		{
			List<RelicApi.RecentMatchHistory.MatchHistoryStat> uniqueMatches = new List<RelicApi.RecentMatchHistory.MatchHistoryStat>();

			var players = Get1v1PlayerList(level, numPlayers);
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
					if (m.maxplayers != 2 || m.description != "AUTOMATCH")
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

		private static List<RelicApi.Leaderboard.Member> Get1v1PlayerList(int level, int numPlayers)
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
