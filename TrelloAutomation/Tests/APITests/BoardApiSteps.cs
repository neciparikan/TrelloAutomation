using TechTalk.SpecFlow;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using TrelloAutomation.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TrelloAutomation.PageObjects;
using TrelloAutomation.Config; 
using Microsoft.Extensions.Options; 
using TrelloAutomation.Models; 
using Microsoft.Extensions.Configuration;


namespace TrelloAutomation.Tests.APITests
{
    [Binding]
    public class BoardApiSteps
    {
        private ApiHelper _apiHelper;
        private string _response;
        private string _boardId;
        private readonly TrelloSettings _trelloSettings; 

        
        public BoardApiSteps(IOptions<TrelloSettings> trelloSettings)
        {
            _trelloSettings = trelloSettings.Value;
        }

        [BeforeScenario]
        public void Setup()
        {
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            // Register TrelloSettings
            services.Configure<TrelloSettings>(configuration.GetSection("Trello"));
            services.AddHttpClient<ApiHelper>();
            services.AddSingleton<ILoginPage, LoginPage>();
            services.AddSingleton<IBoardPage, BoardPage>();

            var serviceProvider = services.BuildServiceProvider();
            _apiHelper = serviceProvider.GetRequiredService<ApiHelper>();
            Log.Information("Setup completed: ApiHelper initialized.");
        }

        [Given(@"I have a valid API key and token")]
        public void GivenIHaveAValidApiKeyAndToken()
        {
            Assert.IsFalse(string.IsNullOrEmpty(_trelloSettings.Credentials.ApiKey), "API Key for creating boards is not set.");
            Assert.IsFalse(string.IsNullOrEmpty(_trelloSettings.Credentials.ApiToken), "API Token for creating boards is not set.");
            Log.Information("Valid API key and token are present.");
        }

        [When(@"I create a board with the name ""(.*)""")]
        public async Task WhenICreateABoardWithTheName(string baseName)
        {
            var boardData = new
            {
                name = baseName,
                defaultLists = true
            };

            string endpoint = "boards/";
            _response = await _apiHelper.PostAsync(endpoint, boardData);
            Log.Information($"Response received for creating board: {_response}");
        }

        [Then(@"I should receive a response indicating the board was created")]
        public void ThenIShouldReceiveAResponseIndicatingTheBoardWasCreated()
        {
            Assert.IsNotNull(_response);
            Assert.IsTrue(_response.Contains("Test Board from API"), "Board was not created successfully.");
            Log.Information("Board creation confirmed in response.");
        }

        [Then(@"the board name should be ""(.*)""")]
        public void ThenTheBoardNameShouldBe(string expectedBaseName)
        {
            Assert.IsTrue(_response.Contains(expectedBaseName), $"Expected board name '{expectedBaseName}' was not found in the response.");
            Log.Information($"Board name validated: {expectedBaseName}");
        }

        [When(@"I retrieve the board ID for ""(.*)""")]
        public async Task WhenIRetrieveTheBoardIDFor(string boardName)
        {
            string endpoint = "members/me/boards";
            var response = await _apiHelper.GetAsync(endpoint);
            Log.Information($"Response received for retrieving boards: {response}");

            var boards = JsonConvert.DeserializeObject<List<Board>>(response);
            Assert.IsNotNull(boards, "Failed to deserialize boards from response.");

            var board = boards.FirstOrDefault(b => b.Name == boardName);
            Assert.IsNotNull(board, $"Board with name '{boardName}' not found.");
            _boardId = board.Id;
            Log.Information($"Board ID retrieved: {_boardId}");
        }

        [Given(@"I have the board ID for ""(.*)""")]
        public void GivenIHaveTheBoardIDFor(string boardName)
        {
            Assert.IsNotNull(_boardId, "Board ID is not set. Please create the board first.");
            Log.Information($"Board ID is available for board: {boardName}");
        }

        [When(@"I create a card with the name ""(.*)"" in the ""(.*)"" list")]
        public async Task WhenICreateACardWithTheNameInTheList(string cardName, string listName)
        {
            var listId = await _apiHelper.GetListIdForBoard(_boardId, listName);
            Assert.IsNotNull(listId, $"List with name '{listName}' not found on board.");

            var cardData = new
            {
                name = cardName,
                idList = listId
            };

            _response = await _apiHelper.PostAsync("cards", cardData);
            Log.Information($"Response received for creating card: {_response}");
        }

        [Then(@"I should receive a response indicating the card was created")]
        public void ThenIShouldReceiveAResponseIndicatingTheCardWasCreated()
        {
            Assert.IsNotNull(_response);
            Assert.IsTrue(_response.Contains("Test API Card"), "Card was not created successfully.");
            Log.Information("Card creation confirmed in response.");
        }

        [Then(@"the card name should be ""(.*)""")]
        public void ThenTheCardNameShouldBe(string expectedCardName)
        {
            Assert.IsTrue(_response.Contains(expectedCardName), $"Expected card name '{expectedCardName}' was not found in the response.");
            Log.Information($"Card name validated: {expectedCardName}");
        }

        [Then(@"I should receive a response containing the board ID")]
        public void ThenIShouldReceiveAResponseContainingTheBoardID()
        {
            Assert.IsFalse(string.IsNullOrEmpty(_boardId), "Board ID should not be null or empty.");
            Log.Information("Board ID is confirmed to be present.");
        }

        [When(@"I get the details of the card with the name ""(.*)""")]
        public async Task WhenIGetTheDetailsOfTheCardWithTheName(string cardName)
        {
            string endpoint = $"boards/{_boardId}/cards";
            var response = await _apiHelper.GetAsync(endpoint);
            Log.Information($"Response received for retrieving cards: {response}");

            var cards = JsonConvert.DeserializeObject<List<Card>>(response);
            var card = cards.FirstOrDefault(c => c.Name == cardName); 

            Assert.IsNotNull(card, $"Card with name '{cardName}' not found.");
            Log.Information($"Card ID retrieved for '{cardName}': {card.Id}"); 

            _response = await _apiHelper.GetAsync($"cards/{card.Id}"); 
        }

        [Then(@"the details should match the expected values")]
        public void ThenTheDetailsShouldMatchTheExpectedValues()
        {
            Assert.IsNotNull(_response);
            var cardDetails = JsonConvert.DeserializeObject<Card>(_response);
            Assert.AreEqual("Test API Card 2", cardDetails.Name, "Card name does not match the expected value.");
            Log.Information($"Card details validated for card: {cardDetails.Name}");
        }

        [Then(@"I should receive a response indicating the card was created with the name ""(.*)""")]
        public void ThenIShouldReceiveAResponseIndicatingTheCardWasCreatedWithTheName(string expectedCardName)
        {
            Assert.IsNotNull(_response);
            Assert.IsTrue(_response.Contains(expectedCardName), $"Card '{expectedCardName}' was not created successfully.");
            Log.Information("Card creation confirmed with expected name in response.");
        }

        [When(@"I update the card status to ""(.*)""")]
        public async Task WhenIUpdateTheCardStatusTo(string newStatus)
        {
            var cardId = await _apiHelper.GetCardIdForBoard(_boardId, "Test API Card for Update");
            Assert.IsNotNull(cardId, "Card ID not found. Ensure the card was created successfully.");

            var listId = await _apiHelper.GetListIdForBoard(_boardId, newStatus);
            Assert.IsNotNull(listId, $"List with name '{newStatus}' not found on board.");

            var updateData = new { idList = listId };
            string updateEndpoint = $"cards/{cardId}";

            var response = await _apiHelper.PutAsync(updateEndpoint, updateData);
            Assert.IsNotNull(response, "Failed to update the card.");
            Log.Information("Card status updated successfully.");
        }

        [Then(@"I should see the card ""(.*)"" present in the ""(.*)"" list \(API\)")]
        public async Task ThenIShouldSeeTheCardPresentInTheListAPI(string cardName, string listName)
        {
            var listId = await _apiHelper.GetListIdForBoard(_boardId, listName);
            Assert.IsNotNull(listId, $"List with name '{listName}' not found on board.");

            var cardsInList = await _apiHelper.GetCardsInList(listId);
            var cardFound = cardsInList.Any(c => c.Name.Equals(cardName, StringComparison.OrdinalIgnoreCase));

            Assert.IsTrue(cardFound, $"The card '{cardName}' was not found in the '{listName}' list.");
            Log.Information($"Card '{cardName}' presence in list '{listName}' confirmed.");
        }

        [When(@"I delete the card with the name ""(.*)""")]
        public async Task WhenIDeleteTheCardWithTheName(string cardName)
        {
            var cardId = await _apiHelper.GetCardIdForBoard(_boardId, cardName);
            Assert.IsNotNull(cardId, $"Card with name '{cardName}' not found.");

            string deleteEndpoint = $"cards/{cardId}";
            _response = await _apiHelper.DeleteAsync(deleteEndpoint);
            Log.Information($"Response received for deleting card: {_response}");
        }

        [Then(@"I should receive a response indicating the card was deleted")]
        public void ThenIShouldReceiveAResponseIndicatingTheCardWasDeleted()
        {
            Assert.IsNotNull(_response);
            Log.Information("Card deletion confirmed in response.");
        }

        [Then(@"the card ""(.*)"" should not be present in the ""(.*)"" list \(API\)")]
        public async Task ThenTheCardShouldNotBePresentInTheListAPI(string cardName, string listName)
        {
            var listId = await _apiHelper.GetListIdForBoard(_boardId, listName);
            Assert.IsNotNull(listId, $"List with name '{listName}' not found on board.");

            var cardsInList = await _apiHelper.GetCardsInList(listId);
            var cardFound = cardsInList.Any(c => c.Name.Equals(cardName, StringComparison.OrdinalIgnoreCase));

            Assert.IsFalse(cardFound, $"The card '{cardName}' was found in the '{listName}' list.");
            Log.Information($"Verified that the card '{cardName}' is not present in the list '{listName}'.");
        }
    }
}
