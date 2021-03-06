using System;

namespace Coh2Stats
{
	public enum CommanderServerId
	{
		AirborneCompany = 186413,
		ArmorCompany = 186415,
		ArmorCompanyDevMEdition = 451587,
		HeavyCavalryCompany = 450482,
		InfantryCompany = 186414,
		MechanizedCompany = 260696,
		ReconSupportCompany = 254449,
		RifleCompany = 261335,
		TacticalSupportCompany = 450479,
		UrbanAssaultCompany = 452456,

		AdvancedEmplacementRegiment = 450553,
		RoyalArtilleryRegiment = 450074,
		CommandoRegiment = 450073,
		RoyalEngineerRegiment = 450075,
		LendLeaseAssaultRegiment = 452455,
		MobileAssaultRegiment = 450554,
		TacticalSupportRegiment = 450268,
		VanguardOperationsRegiment = 450243,
		SpecialWeaponsRegiment = 450286,

		BlitzkriegDoctrine = 6116,
		GermanInfantryDoctrine = 6118,
		GermanMechanizedDoctrine = 6117,
		CloseAirSupportDoctrine = 7554,
		AssaultSupportDoctrine = 5568,
		FestungArmorDoctrine = 5570,
		FestungSupportDoctrine = 5571,
		FortifiedArmorDoctrine = 5927,
		LuftwaffeSupplyDoctrine = 7538,
		DefensiveDoctrineCommunity = 7541,
		EliteTroopsDoctrine = 7537,
		EncirclementDoctrine = 7540,
		MechanizedAssaultDoctrine = 6905,
		MobileDefenseDoctrine = 258891,
		OsttruppenDoctrine = 7249,
		UrbanAssaultDoctrine = 7539,
		JaegerArmorDoctrine = 5572,
		JaegerInfantryDoctrine = 5573,
		JointOperationsDoctrine = 5930,
		LightningWarDoctrine = 5921,
		SpearheadDoctrine = 5928,
		StormDoctrine = 5929,
		StrategicReservesDoctrine = 452454,

		SovietCombinedArmsArmy = 6120,
		SovietReserveArmy = 6121,
		SovietShockArmy = 6119,
		AdvancedWarfareTactics = 186229,
		AirborneTroopsTactics = 452461,
		AntiInfantryTactics = 5926,
		ConscriptSupportTactics = 5922,
		GuardMotorCoordinationTactics = 5575,
		GuardMotorCoordinationTacticsVonIvanEdition = 451588,
		GuardRifleCombinedArmsTactics = 5576,
		MechanizedSupportTactics = 5924,
		NKVDRifleDisruptionTactics = 5578,
		ShockMotorHeavyTactics = 5579,
		ShockRifleFrontlineTactics = 5580,
		DefensiveTacticsCommunity = 7546,
		CounterattackTactics = 7246,
		SovietIndustryTactics = 7543,
		LendLeaseTactics = 258974,
		NotOneStepBackTactics = 7544,
		PartisanTactics = 7542,
		TankHunterTactics = 7545,
		UrbanDefenseTactics = 7247,
		ArmoredAssaultTactics = 5923,
		TerrorTactics = 5925,

		OverwatchDoctrine = 450495,
		BreakthroughDoctrine = 186419,
		EliteArmoredDoctrine = 257800,
		FeuersturmDoctrine = 450494,
		FortificationsDoctrine = 259929,
		GrandOffensiveDoctrine = 452452,
		LuftwaffeGroundForcesDoctrine = 186418,
		ScavengeDoctrine = 259657,
		ScavengeDoctrineJesulinEdition = 451591,
		SpecialOperationsDoctrine = 186417
	}

	[Flags]
	public enum RaceFlag
	{
		None = 0,
		German = 1,
		Soviet = 2,
		WGerman = 4,
		AEF = 8,
		British = 16
	}

	public enum RaceId
	{
		German = 0,
		Soviet = 1,
		WGerman = 2,
		AEF = 3,
		British = 4
	}

	public enum FactionId
	{
		Axis = 0,
		Allies = 1
	}

	public enum MatchTypeId
	{
		_Custom_ = 0,
		_1v1_ = 1,
		_2v2_ = 2,
		_3v3_ = 3,
		_4v4_ = 4,
		_2v2AiEasy_ = 5,
		_2v2AiMedium_ = 6,
		_2v2AiHard_ = 7,
		_2v2AiExpert_ = 8,
		_3v3AiEasy_ = 9,
		_3v3AiMedium_ = 10,
		_3v3AiHard_ = 11,
		_3v3AiExpert_ = 12,
		_4v4AiEasy_ = 13,
		_4v4AiMedium_ = 14,
		_4v4AiHard_ = 15,
		_4v4AiExpert_ = 16,
		_CustomPublic_ = 22
	}

	public enum LeaderboardId
	{
		_CustomGerman_ = 0,
		_CustomSoviet_ = 1,
		_CustomWestGerman_ = 2,
		_CustomAEF_ = 3,
		_1v1German_ = 4,
		_1v1Soviet_ = 5,
		_1v1WestGerman_ = 6,
		_1v1AEF_ = 7,
		_2v2German_ = 8,
		_2v2Soviet_ = 9,
		_2v2WestGerman_ = 10,
		_2v2AEF_ = 11,
		_3v3German_ = 12,
		_3v3Soviet_ = 13,
		_3v3WestGerman_ = 14,
		_3v3AEF_ = 15,
		_4v4German_ = 16,
		_4v4Soviet_ = 17,
		_4v4WestGerman_ = 18,
		_4v4AEF_ = 19,
		_TeamOf2Axis_ = 20,
		_TeamOf2Allies_ = 21,
		_TeamOf3Axis_ = 22,
		_TeamOf3Allies_ = 23,
		_TeamOf4Axis_ = 24,
		_TeamOf4Allies_ = 25,
		_2v2AIEasyAxis_ = 26,
		_2v2AIEasyAllies_ = 27,
		_2v2AIMediumAxis_ = 28,
		_2v2AIMediumAllies_ = 29,
		_2v2AIHardAxis_ = 30,
		_2v2AIHardAllies_ = 31,
		_2v2AIExpertAxis_ = 32,
		_2v2AIExpertAllies_ = 33,
		_3v3AIEasyAxis_ = 34,
		_3v3AIEasyAllies_ = 35,
		_3v3AIMediumAxis_ = 36,
		_3v3AIMediumAllies_ = 37,
		_3v3AIHardAxis_ = 38,
		_3v3AIHardAllies_ = 39,
		_3v3AIExpertAxis_ = 40,
		_3v3AIExpertAllies_ = 41,
		_4v4AIEasyAxis_ = 42,
		_4v4AIEasyAllies_ = 43,
		_4v4AIMediumAxis_ = 44,
		_4v4AIMediumAllies_ = 45,
		_4v4AIHardAxis_ = 46,
		_4v4AIHardAllies_ = 47,
		_4v4AIExpertAxis_ = 48,
		_4v4AIExpertAllies_ = 49,
		_CustomBritish_ = 50,
		_1v1British_ = 51,
		_2v2British_ = 52,
		_3v3British_ = 53,
		_4v4British_ = 54,
	}

	public enum LeaderboardRegionId
	{
		Europe = 0,
		MiddleEast = 1,
		Asia = 2,
		NorthAmerica = 3,
		SouthAmerica = 4,
		Oceania = 5,
		Africa = 6,
		Unknown = 7
	}

	internal class LeaderboardCompatibility
	{
		public static bool LeaderboardBelongsWithMatchType(LeaderboardId leaderboardId, MatchTypeId matchTypeId)
		{
			if (matchTypeId == MatchTypeId._1v1_)
			{
				if (leaderboardId == LeaderboardId._1v1German_) { return true; }
				if (leaderboardId == LeaderboardId._1v1Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._1v1WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._1v1AEF_) { return true; }
				if (leaderboardId == LeaderboardId._1v1British_) { return true; }
			}

			if (matchTypeId == MatchTypeId._2v2_)
			{
				if (leaderboardId == LeaderboardId._2v2German_) { return true; }
				if (leaderboardId == LeaderboardId._2v2Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._2v2WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._2v2AEF_) { return true; }
				if (leaderboardId == LeaderboardId._2v2British_) { return true; }
			}

			if (matchTypeId == MatchTypeId._3v3_)
			{
				if (leaderboardId == LeaderboardId._3v3German_) { return true; }
				if (leaderboardId == LeaderboardId._3v3Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._3v3WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._3v3AEF_) { return true; }
				if (leaderboardId == LeaderboardId._3v3British_) { return true; }
			}

			if (matchTypeId == MatchTypeId._4v4_)
			{
				if (leaderboardId == LeaderboardId._4v4German_) { return true; }
				if (leaderboardId == LeaderboardId._4v4Soviet_) { return true; }
				if (leaderboardId == LeaderboardId._4v4WestGerman_) { return true; }
				if (leaderboardId == LeaderboardId._4v4AEF_) { return true; }
				if (leaderboardId == LeaderboardId._4v4British_) { return true; }
			}

			return false;
		}

		public static LeaderboardId GetLeaderboardFromRaceAndMode(RaceId raceId, MatchTypeId matchTypeId)
		{
			if (raceId == RaceId.German)
			{
				if (matchTypeId == MatchTypeId._1v1_)
				{
					return LeaderboardId._1v1German_;
				}

				if (matchTypeId == MatchTypeId._2v2_)
				{
					return LeaderboardId._2v2German_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					return LeaderboardId._3v3German_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
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
					return LeaderboardId._2v2Soviet_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					return LeaderboardId._3v3Soviet_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
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
					return LeaderboardId._2v2WestGerman_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					return LeaderboardId._3v3WestGerman_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
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
					return LeaderboardId._2v2AEF_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					return LeaderboardId._3v3AEF_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
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
					return LeaderboardId._2v2British_;
				}

				if (matchTypeId == MatchTypeId._3v3_)
				{
					return LeaderboardId._3v3British_;
				}
				if (matchTypeId == MatchTypeId._4v4_)
				{
					return LeaderboardId._4v4British_;
				}
			}

			return LeaderboardId._CustomGerman_;
		}
	}
}
