using TechTalk.SpecFlow;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TrelloAutomation.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TrelloAutomation.PageObjects;
using TrelloAutomation.Config;
using TrelloAutomation.Models;
using Microsoft.Extensions.Configuration;

namespace TrelloAutomation.Tests.APITests
{
    [Binding]
    public class TrelloApiTests
    {
        private ApiHelper _apiHelper;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            services.Configure<TrelloSettings>(configuration.GetSection("Trello"));
            services.AddHttpClient<ApiHelper>();
            services.AddSingleton<ILoginPage, LoginPage>();
            services.AddSingleton<IBoardPage, BoardPage>();

            var serviceProvider = services.BuildServiceProvider();
            _apiHelper = serviceProvider.GetRequiredService<ApiHelper>();
            Log.Information("Setup completed: ApiHelper initialized.");
        }

        private string _boardId;
        private const string BoardName = "Test Board from API";
        private const string CardName = "Test API Card for Deletion";

        [Test]
        public async Task CreateBoard_ShouldReturnCreatedBoard()
        {
            Log.Information("Starting test: CreateBoard_ShouldReturnCreatedBoard");

            var boardData = new
            {
                Name = BoardName, 
                defaultLists = false
            };

            string endpoint = "boards/";
            var response = await _apiHelper.PostAsync(endpoint, boardData);
            Log.Information($"Response received for creating board: {response}");

            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsTrue(response.Contains(BoardName), "Board was not created successfully.");
        }

        [Test]
        public async Task GetBoardId_ShouldReturnBoardId()
        {
            Log.Information("Starting test: GetBoardId_ShouldReturnBoardId");

            string endpoint = "members/me/boards";
            var response = await _apiHelper.GetAsync(endpoint);
            Log.Information($"Response received for getting boards: {response}");

            var boards = JsonConvert.DeserializeObject<List<Board>>(response);
            Assert.IsNotNull(boards, "Failed to deserialize boards from response.");

            var board = boards.FirstOrDefault(b => b.Name == BoardName); 
            Assert.IsNotNull(board, $"Board with name '{BoardName}' not found.");
            _boardId = board.Id; 
            Log.Information($"Board ID retrieved: {_boardId}");
        }

        [Test]
        public async Task CreateList_ShouldReturnCreatedList()
        {
            Log.Information("Starting test: CreateList_ShouldReturnCreatedList");
            await GetBoardId_ShouldReturnBoardId();

            string endpoint = $"boards/{_boardId}/lists";
            var listData = new
            {
                Name = "To Do" 
            };

            var response = await _apiHelper.PostAsync(endpoint, listData);
            Log.Information($"Response received for creating list: {response}");

            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsTrue(response.Contains("To Do"), "List was not created successfully.");
        }

        [Test]
        public async Task CreateCard_ShouldReturnCreatedCard()
        {
            Log.Information("Starting test: CreateCard_ShouldReturnCreatedCard");
            await GetBoardId_ShouldReturnBoardId();

            string endpoint = "cards";
            var cardData = new
            {
                Name = CardName, 
                idList = _boardId
            };

            var response = await _apiHelper.PostAsync(endpoint, cardData);
            Log.Information($"Response received for creating card: {response}");

            Assert.IsNotNull(response, "Response should not be null.");
            Assert.IsTrue(response.Contains(CardName), "Card was not created successfully.");
        }

        [Test]
        public async Task DeleteCard_ShouldReturnDeletedCardResponse()
        {
            Log.Information("Starting test: DeleteCard_ShouldReturnDeletedCardResponse");
            await CreateCard_ShouldReturnCreatedCard();

            var cardId = await _apiHelper.GetCardIdForBoard(_boardId, CardName);
            Assert.IsNotNull(cardId, "Card ID should not be null.");

            string deleteEndpoint = $"cards/{cardId}";
            var deleteResponse = await _apiHelper.DeleteAsync(deleteEndpoint);
            Log.Information($"Response received for deleting card: {deleteResponse}");

            Assert.IsNotNull(deleteResponse, "Response should not be null.");
            Log.Information("Card deletion confirmed in response.");
        }
    }
}
