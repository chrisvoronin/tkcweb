﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TKC.Data;

namespace TKC.Models
{
    public class UserDisplay
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LockOutDate { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class ResourcesPageModel
    {
        public List<KeyValuePair<string, string>> policies = new();
        public List<KeyValuePair<string, string>> governingDocs = new();
    }

    public class TopicsPageModel
    {
        public BlogCategory result;
        public List<BlogCategory> allTopics;
    }


    public class AppSettingModel
    {
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

    public class HTMLContent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
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

    public class PagedResponse<T>
    {
        [JsonPropertyName("totalResults")]
        public int TotalResults { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("itemsPerPage")]
        public int ItemsPerPage { get; set; }

        [JsonPropertyName("items")]
        public List<T> Items { get; set; } = new List<T>();
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

        [JsonPropertyName("singAlongUrl")]
        public string? SingAlongUrl { get; set; }

        [JsonPropertyName("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonPropertyName("aUrl")]
        public string AUrl
        {
            get
            {
                if (AudioUrl == null)
                {
                    return "";
                }

                if (AudioUrl.StartsWith("http"))
                {
                    return AudioUrl;
                }
                return "/File/" + AudioUrl;
            }
        }
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

        [JsonPropertyName("audioFileSize")]
        public long? AudioFileSize { get; set; }

        [JsonPropertyName("audioDuration")]
        public double? AudioDuration { get; set; }

        [JsonPropertyName("aUrl")]
        public string AUrl
        {
            get
            {
                if (AudioUrl == null)
                {
                    return "";
                }

                if (AudioUrl.StartsWith("http"))
                {
                    return AudioUrl;
                }
                return "/File/" + AudioUrl;
            }
        }
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

        [JsonPropertyName("audioFileSize")]
        public long? AudioFileSize { get; set; }

        [JsonPropertyName("audioDuration")]
        public double? AudioDuration { get; set; }

        [JsonPropertyName("pUrl")]
        public string PUrl
        {
            get
            {
                if (PdfUrl == null)
                {
                    return "";
                }

                if (PdfUrl.StartsWith("http"))
                {
                    return PdfUrl;
                }
                return "/File/" + PdfUrl;
            }
        }

        public string ContentType
        {
            get
            {
                if (AudioUrl == null)
                {
                    return "audio/mpeg";
                } else if (AudioUrl.EndsWith("wav"))
                {
                    return "audio/wav";
                } else
                {
                    return "audio/mpeg";
                }
            }
        }

        [JsonPropertyName("aUrl")]
        public string AUrl
        {
            get
            {
                if (AudioUrl == null)
                {
                    return "";
                }

                if (AudioUrl.StartsWith("http"))
                {
                    return AudioUrl;
                }
                return "/File/" + AudioUrl;
            }
        }

        [JsonPropertyName("url")]
        public string Url
        {
            get
            {
                if (VideoUrl == null)
                {
                    return "";
                }

                if (VideoUrl.StartsWith("http"))
                {
                    return VideoUrl;
                }
                return "https://www.youtube.com/embed/" + VideoUrl;
            }
        }
    }

    

    

}

