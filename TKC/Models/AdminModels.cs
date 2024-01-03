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

        [JsonProperty("staffCount")]
        public int StaffCount { get; set; }

        [JsonProperty("settings")]
        public int Settings { get; set; }
	}

    public class StaffViewModel
    {
        public List<Staff> Deacons { get; set; }
        public List<Staff> Elders { get; set; }
        public List<Staff> Secretaries { get; set; }
    }

    public class Staff
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("photoUrl")]
        public string PhotoUrl { get; set; }

        [JsonProperty("order")]
        public long Order { get; set; }
    }
}

