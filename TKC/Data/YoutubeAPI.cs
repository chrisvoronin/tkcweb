using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TKC.Models;

namespace TKC.Data
{

    public class YoutubeAPI
    {
        private const string ApiKey = "AIzaSyA11YMDmXNKwDfJ_qfuXiGnF5q1Kc0zCVk";
        private const string PlaylistUrl = "https://www.googleapis.com/youtube/v3/playlistItems";
        public readonly string PlayListIdSermons = "PLAU3AREAS330OFGY9HP69Xky0MHm4CEGy";

        public async Task<YouTubeApiResponse?> GetPlaylistVideos(CacheService _cache, string playlistId, int perPage, string? pageToken = null)
        {
            var apiUrl = BuildApiUrl(playlistId, perPage, pageToken);
            var apiResponse = _cache.Get<YouTubeApiResponse>(apiUrl);

            if (apiResponse != null)
            {
                return apiResponse;
            }
            else
            {
                using (var httpClient = new HttpClient())
                {
                    try
                    {
                        var response = await httpClient.GetStringAsync(apiUrl);
                        apiResponse = JsonConvert.DeserializeObject<YouTubeApiResponse>(response);
                        if (apiResponse != null)
                        {
                            _cache.Set(apiUrl, apiResponse);
                            foreach (var item in apiResponse.Videos)
                            {
                                _cache.Set(item.Snippet.ResourceId.VideoId, item);
                            }
                        }
                        
                        return apiResponse;
                    }
                    catch (HttpRequestException e)
                    {
                        Console.WriteLine($"Error: {e.Message}");
                        return null; // Return null on error
                    }
                }
            }
        }

        private string BuildApiUrl(string playlistId, int perPage = 10, string? pageToken = null)
        {
            var apiKeyParam = $"key={ApiKey}";
            var playlistIdParam = $"playlistId={playlistId}";
            var partParam = "part=snippet,id";
            var maxResultsParam = $"maxResults={perPage}";
            var pageTokenParam = !string.IsNullOrEmpty(pageToken) ? $"pageToken={pageToken}" : "";
            var orderParam = "order=date"; // Sort by date uploaded

            var apiUrl = $"{PlaylistUrl}?{apiKeyParam}&{playlistIdParam}&{partParam}&{maxResultsParam}&{pageTokenParam}&{orderParam}";

            return apiUrl;
        }

        private string Base64Decode(string base64EncodedData)
        {
            byte[] bytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(bytes);
        }

    }

    public class CacheService
    {
        private readonly IMemoryCache _cache;
        private readonly object _lockObject = new object();
        private readonly MemoryCacheEntryOptions _options = new MemoryCacheEntryOptions
        {
            // Set to expire after x minutes no matter what
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30),

            // Set the sliding expiration time (e.g., cache entry will expire if not accessed for 5 minutes)
            SlidingExpiration = TimeSpan.FromMinutes(10),

            // Set the cache priority
            Priority = CacheItemPriority.Normal
        };

        public CacheService()
        {
            _cache = new MemoryCache(new MemoryCacheOptions());
        }

        public void Set<T>(string key, T value)
        {
            lock (_lockObject)
            {
                if (!_cache.TryGetValue(key, out _))
                {
                    _cache.Set(key, value, _options);
                }
            }
        }

        public T? Get<T>(string key)
        {
            lock (_lockObject)
            {
                return _cache.TryGetValue(key, out T? cachedValue) ? cachedValue : default;
            }
        }

        public void Remove(string key)
        {
            lock (_lockObject)
            {
                _cache.Remove(key);
            }
        }
    }

    public class SermonConverter
    {
        public static SermonListResponse Convert(YouTubeApiResponse response)
        {
            SermonListResponse resp = new SermonListResponse();
            resp.NextPageToken = response.NextPageToken;
            resp.PrevPageToken = response.PrevPageToken;
            resp.ResultsPerPage = response.PageInfo.ResultsPerPage;
            resp.TotalResults = response.PageInfo.TotalResults;
            resp.Sermons = new List<Sermon>();
            foreach (var item in response.Videos)
            {
                var s = Convert(item);
                resp.Sermons.Add(s);
            }
            return resp;
        }

        public static Sermon Convert(YouTubeVideo video)
        {
            string passage = "Passage";
            string speaker = "Pastor";
            string dateCreated = "DateCreated";
            if (video.Snippet.Tags != null && video.Snippet.Tags.Count > 2)
            {
                passage = video.Snippet.Tags[0];
                speaker = video.Snippet.Tags[1];
                dateCreated = video.Snippet.Tags[2];
            }

            var serm = new Sermon()
            {
                Id = video.Snippet.ResourceId.VideoId
                ,
                Title = video.Snippet.Title
                ,
                Passage = passage
                ,
                Speaker = speaker
                ,
                DateCreated = dateCreated
            };

            if (serm.Title.Contains("-"))
            {
                serm.Title = serm.Title.Trim().Split('-', StringSplitOptions.RemoveEmptyEntries).Last().Trim();
                if (serm.Title.Contains(","))
                {
                    serm.Title = serm.Title.Split(',').Last().Trim();
                }
                serm.Title = serm.Title.Replace("\"", "");
            }

            return serm;
        }
    }

}