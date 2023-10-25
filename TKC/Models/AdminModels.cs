using System;
using Newtonsoft.Json;

namespace TKC.Models
{
	public class AdminSummary
	{
        [JsonProperty("sc")]
        public int SermonsCount { get; set; }

        [JsonProperty("mc")]
        public int MusicCount { get; set; }

        [JsonProperty("stc")]
        public int ShortTakeCount { get; set; }
	}
}

