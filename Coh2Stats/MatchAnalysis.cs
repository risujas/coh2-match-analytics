using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MatchAnalysis
	{
		public static void foobar()
		{
			Build1v1PlayerList(1, 100);
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
