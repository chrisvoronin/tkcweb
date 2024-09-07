using System;
using System.ComponentModel.DataAnnotations.Schema;
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

        [JsonProperty("logins")]
        public int Logins { get; set; }

        [JsonProperty("html")]
        public int HtmlContent { get; set; }

        [JsonProperty("resources")]
        public int Resources { get; set; }
    }

    public class StaffViewModel
    {
        public List<Staff> Deacons { get; set; }
        public List<Staff> Elders { get; set; }
        public List<Staff> Pastoral { get; set; }
        public List<Staff> Secretaries { get; set; }
    }

    public class ResourcePageModel
    {
        public List<ResourceGroup> LeftSide = new List<ResourceGroup>();
        public List<ResourceGroup> RightSide = new List<ResourceGroup>();
    }

    public class ResourceDetailPageModel
    {
        public ResourceItem Item { get; set; }
        public List<ResourceGroup> Groups { get; set; }
    }

    public class ResourceGroup
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("side")]
        public int Side { get; set; }

        [JsonProperty("order")]
        public int Order { get; set; }

        [JsonProperty("items")]
        public List<ResourceItem> Items { get; set; } = new List<ResourceItem>();
    }

    public class ResourceItem
    {
        public long Id { get; set; }

        public string FileName { get; set; }

        // ForeignKey attribute to explicitly define the relationship
        [ForeignKey("ResourceGroup")]
        public long GroupId { get; set; }
        public ResourceGroup ResourceGroup { get; set; }

        public string Text { get; set; }

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

