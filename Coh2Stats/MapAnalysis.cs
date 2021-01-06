using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats
{
	class MapAnalysis
	{
		public static Dictionary<string, int> GetMapPopularityDictionary(RaceId raceFlags = RaceId.German | RaceId.Soviet | RaceId.WGerman | RaceId.AEF | RaceId.British)
		{
			Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();

			foreach (var m in MatchHistoryTracker.Matches)
			{
				if (m.HasGivenRaces(raceFlags))
				{
					if (!keyValuePairs.ContainsKey(m.mapname))
					{
						keyValuePairs.Add(m.mapname, 0);
					}

					keyValuePairs[m.mapname] += 1;
				}
			}

			keyValuePairs = keyValuePairs.OrderBy(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
			keyValuePairs = keyValuePairs.Reverse().ToDictionary(p => p.Key, p => p.Value);

			return keyValuePairs;
		}
	}
}
