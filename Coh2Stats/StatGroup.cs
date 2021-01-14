using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Coh2Stats
{
	public class StatGroup
	{
		[JsonProperty("id")] public int Id { get; set; }
		[JsonProperty("name")] public string Name { get; set; }
		[JsonProperty("type")] public int Type { get; set; }
		[JsonProperty("members")] public List<PlayerIdentity> Members { get; set; }
	}
}
