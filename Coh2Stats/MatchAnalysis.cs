using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MatchAnalysis
	{
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

				foreach (var m in response.matchHistoryStats)
				{
					if (m.maxplayers != 2)
					{
						continue;
					}

					bool isUnique = true;
					foreach (var um in uniqueMatches)
					{
						if (um.id == m.id)
						{
							isUnique = false;
							break;
						}
					}

					bool isWithinPermittedTimePeriod = true;
					DateTime cutoffTime = DateTime.Now.AddHours(-maxAgeHours);
					long cutoffUnixTime = ((DateTimeOffset)cutoffTime).ToUnixTimeSeconds();

					Console.WriteLine("cutoff time set to " + cutoffUnixTime);

					if (m.completiontime < cutoffUnixTime)
					{
						isWithinPermittedTimePeriod = false;
					}

					if (isUnique && isWithinPermittedTimePeriod)
					{
						uniqueMatches.Add(m);
					}
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

				var leaderboard = RelicApi.Leaderboard.GetById(i, startRank, numRanks);
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
