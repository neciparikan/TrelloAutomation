using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using TrelloAutomation.Config;
using TrelloAutomation.Models;

namespace TrelloAutomation.Helpers
{
    public class ApiHelper
    {
        private readonly HttpClient _httpClient;
        private readonly TrelloSettings _settings;

        public ApiHelper(HttpClient httpClient, IOptions<TrelloSettings> settings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));

            _httpClient.BaseAddress = new System.Uri("https://api.trello.com/1/");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> PostAsync(string endpoint, object data)
        {
            ValidateSettings();

            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var requestUrl = BuildRequestUrl(endpoint);

            var response = await _httpClient.PostAsync(requestUrl, content);
            await EnsureSuccessResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> DeleteAsync(string endpoint)
        {
            var requestUrl = BuildRequestUrl(endpoint);
            var response = await _httpClient.DeleteAsync(requestUrl);
            await EnsureSuccessResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetAsync(string endpoint)
        {
            var requestUrl = BuildRequestUrl(endpoint);
            var response = await _httpClient.GetAsync(requestUrl);
            await EnsureSuccessResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> PutAsync(string endpoint, object data)
        {
            var json = JsonConvert.SerializeObject(data);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var requestUrl = BuildRequestUrl(endpoint);
            var response = await _httpClient.PutAsync(requestUrl, content);
            await EnsureSuccessResponse(response);
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetBoardsAsync()
        {
            string endpoint = "members/me/boards";
            return await GetAsync(endpoint);
        }

        public async Task<string> GetListIdForBoard(string boardId, string listName)
        {
            string endpoint = $"boards/{boardId}/lists";
            var response = await GetAsync(endpoint);
            var lists = JsonConvert.DeserializeObject<List<TrelloList>>(response); 
            var list = lists.FirstOrDefault(l => l.Name == listName); 
            return list?.Id; 
        }

        public async Task<string> GetCardIdForBoard(string boardId, string cardName)
        {
            string endpoint = $"boards/{boardId}/cards";
            var response = await GetAsync(endpoint);
            var cards = JsonConvert.DeserializeObject<List<Card>>(response);
            var card = cards.FirstOrDefault(c => c.Name == cardName); 
            return card?.Id; 
        }

        public async Task<List<Card>> GetCardsInList(string listId)
        {
            string endpoint = $"lists/{listId}/cards";
            var response = await GetAsync(endpoint);
            return JsonConvert.DeserializeObject<List<Card>>(response);
        }

        private void ValidateSettings()
        {
            if (_settings == null)
                throw new InvalidOperationException("TrelloSettings is not initialized.");
            if (_settings.Credentials == null)
                throw new InvalidOperationException("Credentials are not set in the configuration.");
        }

        private string BuildRequestUrl(string endpoint)
        {
            return $"{endpoint}?key={_settings.Credentials.ApiKey}&token={_settings.Credentials.ApiToken}";
        }

        private async Task EnsureSuccessResponse(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}. Response: {errorContent}");
            }
        }
    }
}
