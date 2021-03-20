namespace Coh2Stats
{
	public class LeaderboardCompatibility
	{
		public static bool LeaderboardIsCompatibleWithGameMode(LeaderboardId leaderboardId, MatchTypeId gameMode)
		{
			if (gameMode == MatchTypeId._1v1_)
			{
				if (leaderboardId == LeaderboardId._1v1German_) { return true; }
				if (leaderboardId == LeaderboardId._1v1Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._1v1WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._1v1AEF_) { return true; }
				if (leaderboardId == LeaderboardId._1v1British_) { return true; }
			}

			if (gameMode == MatchTypeId._2v2_)
			{
				if (leaderboardId == LeaderboardId._2v2German_) { return true; }
				if (leaderboardId == LeaderboardId._2v2Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._2v2WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._2v2AEF_) { return true; }
				if (leaderboardId == LeaderboardId._2v2British_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf2Allies_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf2Axis_) { return true; }
			}

			if (gameMode == MatchTypeId._3v3_)
			{
				if (leaderboardId == LeaderboardId._3v3German_) { return true; }
				if (leaderboardId == LeaderboardId._3v3Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._3v3WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._3v3AEF_) { return true; }
				if (leaderboardId == LeaderboardId._3v3British_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf3Allies_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf3Axis_) { return true; }
			}

			if (gameMode == MatchTypeId._4v4_)
			{
				if (leaderboardId == LeaderboardId._4v4German_) { return true; }
				if (leaderboardId == LeaderboardId._4v4Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._4v4WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._4v4AEF_) { return true; }
				if (leaderboardId == LeaderboardId._4v4British_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf4Allies_) { return true; }
				if (leaderboardId == LeaderboardId._TeamOf4Axis_) { return true; }
			}

			return false;
		}

		public static LeaderboardId GetLeaderboardFromRaceAndMode(RaceId raceId, MatchTypeId matchTypeId, bool arrangedTeam)
		{
			if (raceId == RaceId.German)
			{
				if (matchTypeId == MatchTypeId._1v1_)
				{
					return LeaderboardId._1v1German_;
				}

				if (matchTypeId == MatchTypeId._2v2_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf2Axis_;
					}
					return LeaderboardId._2v2German_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf3Axis_;
					}
					return LeaderboardId._3v3German_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf4Axis_;
					}
					return LeaderboardId._4v4German_;
				}
			}

			if (raceId == RaceId.Soviet)
			{
				if (matchTypeId == MatchTypeId._1v1_)
				{
					return LeaderboardId._1v1Soviet_;
				}

				if (matchTypeId == MatchTypeId._2v2_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf2Allies_;
					}
					return LeaderboardId._2v2Soviet_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf3Allies_;
					}
					return LeaderboardId._3v3Soviet_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf4Allies_;
					}
					return LeaderboardId._4v4Soviet_;
				}
			}

			if (raceId == RaceId.WGerman)
			{
				if (matchTypeId == MatchTypeId._1v1_)
				{
					return LeaderboardId._1v1WestGerman_;
				}

				if (matchTypeId == MatchTypeId._2v2_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf2Axis_;
					}
					return LeaderboardId._2v2WestGerman_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf3Axis_;
					}
					return LeaderboardId._3v3WestGerman_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf4Axis_;
					}
					return LeaderboardId._4v4WestGerman_;
				}
			}

			if (raceId == RaceId.AEF)
			{
				if (matchTypeId == MatchTypeId._1v1_)
				{
					return LeaderboardId._1v1AEF_;
				}

				if (matchTypeId == MatchTypeId._2v2_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf2Allies_;
					}
					return LeaderboardId._2v2AEF_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf3Allies_;
					}
					return LeaderboardId._3v3AEF_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf4Allies_;
					}
					return LeaderboardId._4v4AEF_;
				}
			}

			if (raceId == RaceId.British)
			{
				if (matchTypeId == MatchTypeId._1v1_)
				{
					return LeaderboardId._1v1British_;
				}

				if (matchTypeId == MatchTypeId._2v2_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf2Allies_;
					}
					return LeaderboardId._2v2British_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf3Allies_;
					}
					return LeaderboardId._3v3British_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
					if (arrangedTeam)
					{
						return LeaderboardId._TeamOf4Allies_;
					}
					return LeaderboardId._4v4British_;
				}
			}

			return LeaderboardId._CustomGerman_;
		}
	}
}
