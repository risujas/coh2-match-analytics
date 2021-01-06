using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coh2Stats.RelicApi
{
	class Utilities
	{
		// Takes in a list of Steam64IDs: ["76561198050674754", "76561198050674755"]
		// Outputs: "/steam/76561198050674754", "/steam/76561198050674755"
		public static string FormatListToSteamIdString(List<string> steamIds)
		{
			string idString = "";
			for (int i = 0; i < steamIds.Count; i++)
			{
				idString += "\"/steam/";
				idString += steamIds[i];
				idString += "\"";

				if (steamIds.Count > i + 1)
				{
					idString += ", ";
				}
			}
			return idString;
		}
	}
}
