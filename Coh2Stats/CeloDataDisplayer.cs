using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class CeloDataDisplayer
	{
		public void init()
		{
			var players = LogParser.GetPlayerList();

			foreach (var p in players)
			{
				var root = PersonalStatResponse.GetPersonalStatByProfileId(p.profileId);
				int statGroupId = -1;

				foreach (var stg in root.statGroups)
				{
					if (stg.type == 1)
					{
						statGroupId = stg.id;
					}
				}

				int leaderboardId = -1;
				if (p.race == "soviet")
				{
					leaderboardId = 5;
				}
				if (p.race == "german")
				{
					leaderboardId = 4;
				}
				if (p.race == "west_german")
				{
					leaderboardId = 6;
				}
				if (p.race == "aef")
				{
					leaderboardId = 7;
				}
				if (p.race == "british")
				{
					leaderboardId = 51;
				}

				foreach (var lbs in root.leaderboardStats)
				{
					if (lbs.statGroup_id == statGroupId && lbs.leaderboard_id == leaderboardId)
					{
						p.rank = lbs.rank;
					}
				}

				Console.WriteLine(p.nickName + ": " + p.race + " " + p.rank);
			}
		}
	}
}
