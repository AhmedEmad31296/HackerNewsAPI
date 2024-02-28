using Microsoft.AspNetCore.Mvc;

using System.Text.Json;

namespace HackerNewsAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HackerNewsController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public HackerNewsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        public async Task<IEnumerable<StoryInfoDto>> GetBestStories(int n)
        {
            string IDsURL = "https://hacker-news.firebaseio.com/v0/beststories.json";

            // Get the IDs of the best stories
            var bestStoryIdsResponse = await _httpClient.GetAsync(IDsURL);
            bestStoryIdsResponse.EnsureSuccessStatusCode();
            var bestStoryIds = await JsonSerializer.DeserializeAsync<List<int>>(await bestStoryIdsResponse.Content.ReadAsStreamAsync());

            // Retrieve details of the best n stories
            var bestStories = new List<StoryInfoDto>();
            foreach (var storyId in bestStoryIds.Take(n))
            {
                var storyResponse = await _httpClient.GetAsync($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json");
                storyResponse.EnsureSuccessStatusCode();
                var story = await JsonSerializer.DeserializeAsync<StoryInfoDto>(await storyResponse.Content.ReadAsStreamAsync());
                bestStories.Add(story);
            }

            // Order the stories by score in descending order
            bestStories = bestStories.OrderByDescending(s => s.score).ToList();

            return bestStories;
        }
    }


}