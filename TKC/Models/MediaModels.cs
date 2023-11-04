using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
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

    public class SermonListResponse
    {
        public string NextPageToken { get; set; }
        public string PrevPageToken { get; set; }
        public int TotalResults { get; set; }
        public int ResultsPerPage { get; set; }
        public List<Sermon> Sermons { get; set; }
        public int CurrentPage { get; set; } = 1;
    }

    public class ShortTakeResponse
    {
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("items")]
        public List<ShortTake> Items { get; set; } = new List<ShortTake>();
    }

    public class EmployeeResponse
    {
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("items")]
        public List<Staff> Items { get; set; } = new List<Staff>();
    }

    public class SermonsResponse
    {
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("items")]
        public List<Sermon> Items { get; set; } = new List<Sermon>();
    }

    public class MusicResponse
    {
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("items")]
        public List<Music> Items { get; set; } = new List<Music>();
    }

    public class ResourcesViewModel
    {
        public required List<Music> Music { get; set; }
        public required List<Sermon> Sermons { get; set; }
        public required List<ShortTake> Shorts { get; set; }
    }

	public class Music
	{
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("subTitle")]
        public string? SubTitle { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("audioUrl")]
        public string? AudioUrl { get; set; }

        [JsonPropertyName("pdfUrl")]
        public string? PdfUrl { get; set; }

        [JsonPropertyName("videoUrl")]
        public string? VideoUrl { get; set; }

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }
    }

	public class ShortTake
	{
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("subTitle")]
        public string? SubTitle { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("fileUrl")]
        public string? AudioUrl { get; set; }

        [JsonPropertyName("pdfUrl")]
        public string? PdfUrl { get; set; }

        [JsonPropertyName("videoUrl")]
        public string? VideoUrl { get; set; }

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }
    }

    public class Sermon
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("subTitle")]
        public string? SubTitle { get; set; }

        [JsonPropertyName("author")]
        public string? Author { get; set; }

        [JsonPropertyName("fileUrl")]
        public string? AudioUrl { get; set; }

        [JsonPropertyName("pdfUrl")]
        public string? PdfUrl { get; set; }

        [JsonPropertyName("videoUrl")]
        public string? VideoUrl { get; set; }

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("url")]
        public string Url
        {
            get
            {
                return "https://www.youtube.com/embed/" + VideoUrl ?? "";
            }
        }
    }

    

    

}

