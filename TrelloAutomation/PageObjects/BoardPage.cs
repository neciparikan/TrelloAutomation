using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using System.Linq;

namespace TrelloAutomation.PageObjects
{
    public class BoardPage : IBoardPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        private readonly By _createBoardButton = By.XPath("//li[@data-testid='create-board-tile']//span[contains(text(),'Create new board')]");
        private readonly By _boardNameInput = By.XPath("//input[@data-testid='create-board-title-input']");
        private readonly By _createButton = By.XPath("//button[@data-testid='create-board-submit-button']");
        private readonly By _boardTile = By.ClassName("board-tile");
        private readonly By _errorMessage = By.XPath("//div[contains(text(),'Board name is required')]");

        private readonly By _toDoAddCardButton = By.XPath("//h2[text()='To Do']/ancestor::div[@data-testid='list-header']/following-sibling::div[@data-testid='list-footer']//button[@data-testid='list-add-card-button']");
        private readonly By _cardInput = By.XPath("//textarea[@data-testid='list-card-composer-textarea']");
        private readonly By _saveButton = By.XPath("//button[@data-testid='list-card-composer-add-card-button']");

        public BoardPage(IWebDriver driver)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
        }

        public void ClickCreateBoardButton()
        {
            Log.Information("Clicking 'Create new Board' button.");
            var createBoardButton = _wait.Until(driver => driver.FindElement(_createBoardButton));
            createBoardButton.Click();
        }

        public void EnterBoardName(string boardName)
        {
            Log.Information("Entering board name.");
            var boardNameInput = _wait.Until(driver => driver.FindElement(_boardNameInput));
            boardNameInput.Clear();
            boardNameInput.SendKeys(boardName);
        }

        public void ClickCreateButton()
        {
            Log.Information("Clicking 'Create Board' button.");
            var createButtonElement = _driver.FindElement(_createButton);
            createButtonElement.Click();
        }

        public bool IsBoardDisplayed(string boardName)
        {
            try
            {
                var boards = _driver.FindElements(_boardTile);
                return boards.Any(b => b.Text.Contains(boardName));
            }
            catch (Exception ex)
            {
                Log.Error($"Error while checking if the board '{boardName}' is displayed: {ex.Message}");
                return false;
            }
        }


        public bool IsErrorMessageDisplayed()
        {
            try
            {
                return _driver.FindElement(_errorMessage).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void ClickAddCardButton()
        {
            Log.Information("Clicking 'Add a card' button in 'To Do' section.");
            _driver.Navigate().Refresh();
            var addCardButtonElement = _wait.Until(driver => driver.FindElement(_toDoAddCardButton));
            addCardButtonElement.Click();
        }

        public void EnterCardName(string cardName)
        {
            Log.Information("Entering card name.");
            var cardInput = _wait.Until(driver => driver.FindElement(_cardInput));
            cardInput.Clear();
            cardInput.SendKeys(cardName);
        }

        public void ClickSaveButton()
        {
            Log.Information("Clicking 'Add card' button.");
            var saveButton = _driver.FindElement(_saveButton);
            saveButton.Click();
        }

        public bool IsCardDisplayed(string cardName)
        {
            Log.Information($"Checking if the card '{cardName}' is displayed.");
            var cards = _driver.FindElements(By.XPath("//ol[@data-testid='list-cards']//a[@data-testid='card-name']"));
            return cards.Any(c => c.Text.Equals(cardName, StringComparison.OrdinalIgnoreCase));
        }

        public bool IsCardDisplayedInList(string cardName, string listName)
        {
            Log.Information($"Checking if the card '{cardName}' is displayed in the '{listName}' list.");
            var cardsInList = _driver.FindElements(By.XPath($"//h2[text()='{listName}']/ancestor::div[@data-testid='list']//a[contains(@data-testid, 'card-name') and text()='{cardName}']"));
            return cardsInList.Any();
        }
    }
}
