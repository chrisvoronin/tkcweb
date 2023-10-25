using Newtonsoft.Json;
using System.Collections.Generic;

namespace TKC.Data
{
    public class YouTubeVideo
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("etag")]
        public string Etag { get; set; }

        [JsonProperty("snippet")]
        public VideoSnippet Snippet { get; set; }
    }

    public class VideoSnippet
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("thumbnails")]
        public Thumbnails Thumbnails { get; set; }

        [JsonProperty("tags")]
        public List<string> Tags { get; set; }

        [JsonProperty("resourceId")]
        public ResourceId ResourceId { get; set; }
    }

    public class ResourceId
    {
        [JsonProperty("kind")]
        public string Kind { get; set; }

        [JsonProperty("videoId")]
        public string VideoId { get; set; }
    }

    public class Thumbnails
    {
        [JsonProperty("default")]
        public Thumbnail Default { get; set; }

        [JsonProperty("medium")]
        public Thumbnail Medium { get; set; }

        [JsonProperty("high")]
        public Thumbnail High { get; set; }
    }

    public class Thumbnail
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }
    }

    public class PageInfo
    {
        [JsonProperty("totalResults")]
        public int TotalResults { get; set; }

        [JsonProperty("resultsPerPage")]
        public int ResultsPerPage { get; set; }
    }

    // This class represents the root of the response
    public class YouTubeApiResponse
    {
        [JsonProperty("items")]
        public List<YouTubeVideo> Videos { get; set; }

        [JsonProperty("nextPageToken")]
        public string NextPageToken { get; set; }

        [JsonProperty("prevPageToken")]
        public string PrevPageToken { get; set; }

        [JsonProperty("pageInfo")]
        public PageInfo PageInfo { get; set; }
    }

}
