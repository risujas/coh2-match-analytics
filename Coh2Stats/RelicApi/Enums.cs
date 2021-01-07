using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats.RelicApi
{
	[Flags]
	enum RaceFlag
	{
		German = 1,
		Soviet = 2,
		WGerman = 4,
		AEF = 8,
		British = 16
	}

	enum RaceId
	{
		German = 0,
		Soviet = 1,
		WGerman = 2,
		AEF = 3,
		British = 4
	}

	enum FactionId
	{
		Axis = 0,
		Allies = 1
	}

	enum GameModeId
	{
		OneVsOne = 1,
		TwoVsTwo = 2,
		ThreeVsThree = 3,
		FourVsFour = 4
	}
}
