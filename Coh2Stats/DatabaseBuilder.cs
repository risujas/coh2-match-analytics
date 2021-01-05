using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Coh2Stats
{
	class Player
	{
		public string Alias;
		public string ProfileId;

		public Player(string alias, string profileId)
		{
			Alias = alias;
			ProfileId = profileId;
		}
	}

	class MatchDataSet
	{
		public RelicApi.RecentMatchHistory.MatchHistoryStat MatchHistoryStat = new RelicApi.RecentMatchHistory.MatchHistoryStat();
		public string Id = "";
	}

	class DatabaseBuilder
	{
		private List<Player> Players = new List<Player>();
		private List<MatchDataSet> Matches = new List<MatchDataSet>();

		public bool PlayerListContainsEntry(string profileId)
		{
			foreach (var p in Players)
			{
				if (p.ProfileId == profileId)
				{
					return true;
				}
			}

			return false;
		}

		public string GetPlayerAlias(string profileId)
		{
			string alias = "";

			foreach (var p in Players)
			{
				if (p.ProfileId == profileId)
				{
					alias = p.Alias;
					break;
				}
			}

			// maybe throw exception here

			return alias;
		}

		public void BuildPlayerList()
		{
			Players.Clear();

			for (int i = 4; i <= 51; i++)
			{
				if (i > 7 && i < 51)
				{
					continue;
				}

				var lb = RelicApi.Leaderboard.GetById(i, 1, 10);
				List<Player> tempPlayersList = new List<Player>();

				foreach (var sg in lb.statGroups)
				{
					string profileId = sg.members[0].profile_id.ToString();
					if (!PlayerListContainsEntry(profileId))
					{
						tempPlayersList.Add(new Player(sg.members[0].alias, profileId));
					}
				}

				Players.AddRange(tempPlayersList);
			}
		}

		public void BuildMatchList(int ageLimitInHours = 24)
		{
			int duplicateGames = 0;

			foreach (var player in Players)
			{
				var recentMatchHistory = RelicApi.RecentMatchHistory.GetByProfileId(player.ProfileId);
				foreach (var mhs in recentMatchHistory.matchHistoryStats)
				{
					DateTime dt = DateTime.Now.AddHours(-ageLimitInHours);
					long cutoffTime = ((DateTimeOffset)dt).ToUnixTimeSeconds();

					if (mhs.maxplayers != 2 || mhs.description != "AUTOMATCH" || mhs.completiontime < cutoffTime)
					{
						continue;
					}

					MatchDataSet matchDataSet = new MatchDataSet();
					matchDataSet.MatchHistoryStat = mhs;
					matchDataSet.Id = mhs.id.ToString() + "_" + mhs.completiontime.ToString();

					bool alreadyLogged = false;

					foreach (var m in Matches)
					{
						if (m.Id == matchDataSet.Id)
						{
							alreadyLogged = true;
							break;
						}
					}

					if (alreadyLogged)
					{
						duplicateGames++;
						continue;
					}

					Matches.Add(matchDataSet);
				}
				
			}

			Matches = Matches.OrderBy(m => m.MatchHistoryStat.completiontime).ToList();
			Matches.Reverse();
		}

		public void PrintMatchList()
		{
			foreach (var m in Matches)
			{
				Console.WriteLine("----------------------------------------------------------------");
				Console.WriteLine(new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(m.MatchHistoryStat.completiontime).ToString());
				Console.WriteLine(m.MatchHistoryStat.mapname);

				foreach (var reportResults in m.MatchHistoryStat.matchhistoryreportresults)
				{
					Console.WriteLine("{0} {1} {2}", 
						Utilities.GetAliasByProfileId(reportResults.profile_id.ToString()), 
						Utilities.GetHumanReadableRaceId(reportResults.race_id), 
						Utilities.GetHumanReadableResultType(reportResults.resulttype));
				}
			}
		}
	}
}
