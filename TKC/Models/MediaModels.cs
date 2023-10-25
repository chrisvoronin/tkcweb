using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using TKC.Data;

namespace TKC.Models
{
    public class AppSettingModel
    {
        public long Id { get; set; }
        public required string Key { get; set; }
        public required string Value { get; set; }
    }
    public class MusicViewModel
    {
        public int TotalCount { get; set; }
        public int PageCount { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage { get; set; }
        public int PreviousPage { get; set; }
        public int NextPage { get; set; }
        public required List<Music> Data { get; set; }
    }

    public class ResourcesViewModel
    {
        public required List<Music> Music { get; set; }
        public required List<Sermon> Sermons { get; set; }
        public required List<ShortTake> Shorts { get; set; }
    }

	public class Music
	{
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public required string Title { get; set; }

        [JsonProperty("subTitle")]
        public string? SubTitle { get; set; }

        [JsonProperty("author")]
        public string? Author { get; set; }

        [JsonProperty("fileUrl")]
        public string? FileUrl { get; set; }

        [JsonProperty("pdfUrl")]
        public string? PdfUrl { get; set; }

        [JsonProperty("videoUrl")]
        public string? VideoUrl { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }
    }

	public class ShortTake
	{
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("title")]
        public string? Title { get; set; }

        [JsonProperty("speaker")]
        public string? Speaker { get; set; }

        [DataType(DataType.DateTime)] // Specify the data type
        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("url")]
        public string? Url { get; set; }
    }

    public class ShortTakeResponse
    {
        [JsonProperty("TotalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("currentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonProperty("items")]
        public List<ShortTake> Items { get; set; } = new List<ShortTake>();
    }

    public class SermonListResponse
    {
        public string NextPageToken { get; set; }
        public string PrevPageToken { get; set; }
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
        public List<Sermon> Sermons { get; set; }
        public int CurrentPage { get; set; } = 1;
    }

    public class Sermon
    {
        public required string Id { get; set; }
        public string? Title { get; set; }
        public string? Speaker { get; set; }
        public string? Passage { get; set; }
        public string? DateCreated { get; set; }
        public string Url
        {
            get
            {
                return "https://www.youtube.com/embed/" + Id;
            }
        }
    }

}

