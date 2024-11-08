using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using System;
using TechTalk.SpecFlow;
using TrelloAutomation.Config;
using TrelloAutomation.Helpers;
using TrelloAutomation.PageObjects;
using Microsoft.Extensions.Options;

namespace TrelloAutomation.Tests.UITests
{
    [Binding]
    public class BoardSteps
    {
        private IWebDriver _driver;
        private ILoginPage _loginPage;
        private BoardPage _boardPage;
        private readonly string _createdBoardName = "Test";
        private readonly TrelloSettings _trelloSettings; 

       
        public BoardSteps(IOptions<TrelloSettings> trelloSettings)
        {
            _trelloSettings = trelloSettings.Value; 
        }

        [BeforeScenario]
        public void Setup()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            _driver = WebDriverFactory.CreateDriver();
            _loginPage = new LoginPage(_driver, Options.Create(_trelloSettings)); 
            _boardPage = new BoardPage(_driver);
        }

        // Login Scenarios
        [Given(@"I navigate to the Trello home page")]
        public void GivenINavigateToTheTrelloHomePage()
        {
            _driver.Navigate().GoToUrl(_trelloSettings.Url); 
            Log.Information("Navigated to Trello home page at {TrelloUrl}", _trelloSettings.Url);
        }

        [Then(@"I should see the title ""(.*)""")]
        public void ThenIShouldSeeTheTitle(string expectedTitle)
        {
            string fullExpectedTitle = $"{expectedTitle} | Trello";
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Title.Contains(fullExpectedTitle));
            Assert.That(_driver.Title, Is.EqualTo(fullExpectedTitle), "The title does not match the expected full title.");
            Log.Information("Verified the page title: {Title}", _driver.Title);
        }

        [When(@"I click the ""Log in"" button")]
        public void WhenIClickTheLoginButton()
        {
            _driver.FindElement(By.LinkText("Log in")).Click();
            Log.Information("Clicked the 'Log in' button.");
        }

        [When(@"I click the ""Continue"" button")]
        public void WhenIClickTheContinueButton()
        {
            _driver.FindElement(By.Id("login-submit")).Click();
            Log.Information("Clicked the 'Continue' button.");
        }

        [When(@"I enter my email")]
        public void WhenIEnterMyEmail()
        {
            _loginPage.EnterEmail(_trelloSettings.Credentials.ValidEmail); 
            Log.Information("Entered email: {Email}", _trelloSettings.Credentials.ValidEmail);
        }

        [When(@"I enter my password")]
        public void WhenIEnterMyPassword()
        {
            _loginPage.EnterPassword(_trelloSettings.Credentials.ValidPassword);
            Log.Information("Entered password.");
        }

        [When(@"I submit login")]
        public void WhenISubmitLogin()
        {
            _loginPage.ClickLogin();
            Log.Information("Submitted login form.");
        }

        [When(@"I enter wrong password")]
        public void WhenIEnterWrongPassword()
        {
            _loginPage.EnterPassword(_trelloSettings.Credentials.InvalidPassword); 
            Log.Information("Entered invalid password for testing.");
        }

        [When(@"I continue without two-step verification if the button is visible")]
        public void WhenIContinueWithoutTwoStepVerificationIfVisible()
        {
            var continueButton = _driver.FindElements(By.XPath("//button[contains(text(),'Continue without two-step verification')]"));
            if (continueButton.Count > 0 && continueButton[0].Displayed)
            {
                continueButton[0].Click();
                Log.Information("Clicked on 'Continue without two-step verification' button.");
            }
            else
            {
                Log.Information("Continue without two-step verification button not visible.");
            }
        }

        [Then(@"I should be redirected to the Trello dashboard")]
        public void ThenIShouldBeRedirectedToTheTrelloDashboard()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Title.Contains("Boards | Trello"));
            Assert.That(_driver.Title, Is.EqualTo("Boards | Trello"), "The title does not match the expected title.");
            Log.Information("Successfully redirected to the Trello dashboard.");
        }

        [Then(@"I should see an error message containing ""(.*)""")]
        public void ThenIShouldSeeAnErrorMessageContaining(string expectedErrorSubstring)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.FindElement(By.XPath("//section[@data-testid='form-error']")).Displayed);
            var errorMessage = _loginPage.GetErrorMessage();
            Assert.That(errorMessage, Does.Contain(expectedErrorSubstring), "The expected error message was not found.");
            Log.Information("Verified error message containing: {ExpectedErrorSubstring}", expectedErrorSubstring);
        }

        // Board Management Scenarios
        [Given(@"I am logged into Trello")]
        public void GivenIAmLoggedIntoTrello()
        {
            GivenINavigateToTheTrelloHomePage();
            WhenIClickTheLoginButton();
            WhenIEnterMyEmail();
            WhenIClickTheContinueButton();
            WhenIEnterMyPassword();
            WhenISubmitLogin();
            WhenIContinueWithoutTwoStepVerificationIfVisible();
            Log.Information("User logged into Trello successfully.");
        }

        [When(@"I click on the ""Create new board"" button")]
        public void WhenIClickOnTheCreateNewBoardButton()
        {
            _boardPage.ClickCreateBoardButton();
            Log.Information("Clicked the 'Create new board' button.");
        }

        [When(@"I enter a valid board name ""(.*)""")]
        public void WhenIEnterAValidBoardName(string boardName)
        {
            _boardPage.EnterBoardName(boardName);
            Log.Information("Created board with name: {BoardName}", boardName);
        }

        [When(@"I click the ""Create Board"" button")]
        public void WhenIClickTheCreateBoardButton()
        {
            _boardPage.ClickCreateButton();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Title.Contains($"{_createdBoardName} | Trello"));
            Log.Information("Board created and redirected to board page.");
        }

        [Then(@"I should see the newly created board in my list of boards")]
        public void ThenIShouldSeeTheNewlyCreatedBoardInMyListOfBoards()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => driver.Title.Contains($"{_createdBoardName} | Trello"));
            Assert.That(_driver.Title, Is.EqualTo($"{_createdBoardName} | Trello"), "The board title does not match.");
            Log.Information("Verified the newly created board title: {BoardTitle}", _createdBoardName);
        }

        [When(@"I click on the ""(.*)"" board")]
        public void WhenIClickOnTheBoard(string boardName)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var boardElement = wait.Until(driver =>
                driver.FindElement(By.XPath($"//a[contains(@class, 'board-tile') and contains(., '{boardName}')]"))
            );

            boardElement.Click();
            Log.Information("Clicked on the board: {BoardName}", boardName);
        }

        [When(@"I click the card input field for ""(.*)""")]
        public void WhenIClickTheCardInputFieldFor(string cardName)
        {
            _driver.Navigate().Refresh();
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var cardElement = wait.Until(driver =>
                driver.FindElement(By.XPath($"//a[contains(@data-testid, 'card-name') and text()='{cardName}']"))
            );

            cardElement.Click();
            Log.Information("Clicked on the card: {CardName}", cardName);
        }

        [Then(@"I should see the information containing ""(.*)""")]
        public void ThenIShouldSeeTheInformationContaining(string expectedInfo)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            var cardBackAction = wait.Until(driver =>
                driver.FindElement(By.XPath("//div[@data-testid='card-back-action']"))
            );

            Assert.IsTrue(cardBackAction.Text.Contains(expectedInfo), $"Expected information was not found: '{expectedInfo}'.");
            Log.Information("Verified information on the card contains: {ExpectedInfo}", expectedInfo);
        }

        [When(@"I click the ""Add a card"" button")]
        public void WhenIClickTheAddACardButton()
        {
            _boardPage.ClickAddCardButton();
            Log.Information("Clicked the 'Add a card' button.");
        }

        [When(@"I enter the card name ""(.*)""")]
        public void WhenIEnterTheCardName(string cardName)
        {
            _boardPage.EnterCardName(cardName);
            Log.Information("Entered card name: {CardName}", cardName);
        }

        [When(@"I click the ""Save"" button to create the card")]
        public void WhenIClickTheSaveButtonToCreateTheCard()
        {
            _boardPage.ClickSaveButton();
            Log.Information("Clicked the 'Save' button to create the card.");
        }

        [Then(@"I should see the card ""(.*)"" in the list of cards")]
        public void ThenIShouldSeeTheCardInTheListOfCards(string cardName)
        {
            Assert.IsTrue(_boardPage.IsCardDisplayed(cardName), $"The card '{cardName}' was not found in the list.");
            Log.Information("Verified that the card '{CardName}' is displayed in the list.", cardName);
        }

        [When(@"I click the ""To Do"" dropdown button")]
        public void WhenIClickTheToDoButton()
        {
            Log.Information("Attempting to click the 'To Do' dropdown button.");
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));

            var toDoDropdownButton = wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(By.XPath("//button[@data-testid='card-back-move-card-button']"));
                    return element.Enabled ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            if (toDoDropdownButton != null)
            {
                toDoDropdownButton.Click();
                Log.Information("Clicked the 'To Do' dropdown button.");
            }
            else
            {
                Log.Error("The 'To Do' dropdown button is not found or is disabled.");
                throw new Exception("The 'To Do' dropdown button is not clickable or not found.");
            }
        }

        [When(@"I select ""(.*)""")]
        public void WhenISelect(string status)
        {
            Log.Information($"Attempting to select '{status}' from the dropdown.");
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(20));

            var dropdown = wait.Until(driver =>
            {
                try
                {
                    var element = driver.FindElement(By.XPath("//div[@data-testid='move-card-popover-select-list-destination']"));
                    return element.Displayed ? element : null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });

            if (dropdown != null)
            {
                dropdown.Click();
                Log.Information("Clicked the dropdown to open options.");

                var actions = new OpenQA.Selenium.Interactions.Actions(_driver);
                actions.SendKeys(Keys.ArrowDown) 
                       .SendKeys(Keys.Enter) 
                       .Perform();

                Log.Information($"Selected '{status}' from the dropdown using keyboard navigation.");
            }
            else
            {
                Log.Error("The dropdown is not found or is disabled.");
                throw new Exception("The dropdown for selecting status is not clickable or not found.");
            }
        }

        [When(@"I click the ""Move"" button")]
        public void WhenIClickTheMoveButton()
        {
            var moveButton = _driver.FindElement(By.XPath("//button[@data-testid='move-card-popover-move-button']"));
            moveButton.Click();
            Log.Information("Clicked the 'Move' button.");
        }

        [Then(@"I should see the card ""(.*)"" in the ""(.*)"" list")]
        public void ThenIShouldSeeTheCardInTheList(string cardName, string listName)
        {
            Assert.IsTrue(_boardPage.IsCardDisplayedInList(cardName, listName), $"The card '{cardName}' was not found in the '{listName}' list.");
            Log.Information("Verified that the card '{CardName}' is displayed in the list '{ListName}'.", cardName, listName);
        }

        [When(@"I click on the ""(.*)"" card in the ""(.*)"" section")]
        public void WhenIClickOnTheCardInSection(string cardName, string sectionName)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var cardElement = wait.Until(driver =>
                driver.FindElement(By.XPath($"//div[@class='list' and contains(., '{sectionName}')]//div[contains(@class, 'list-card') and contains(., '{cardName}')]"))
            );

            cardElement.Click();
            Log.Information("Clicked on the card: {CardName} in section: {SectionName}", cardName, sectionName);
        }

        [Then(@"I should see ""(.*)""")]
        public void ThenIShouldSee(string expectedText)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(30));
            var cardBackAction = wait.Until(driver =>
                driver.FindElement(By.XPath("//div[@data-testid='card-back-action']"))
            );

            Assert.IsTrue(cardBackAction.Text.Contains(expectedText), $"Expected information was not found: '{expectedText}'.");
            Log.Information("Verified information on the card contains: {ExpectedText}", expectedText);
        }

        [When(@"I close the card details window")]
        public void WhenICloseTheCardDetailsWindow()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var closeButton = wait.Until(driver =>
                driver.FindElement(By.XPath("//button[@aria-label='Close dialog']"))
            );

            closeButton.Click();
            Log.Information("Closed the card details window.");
        }

        [When(@"I click the ""Archive"" button")]
        public void WhenIClickTheArchiveButton()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var archiveButton = wait.Until(driver =>
                driver.FindElement(By.XPath("//button[@data-testid='card-back-archive-button']"))
            );
            archiveButton.Click();
            Log.Information("Clicked the 'Archive' button.");
        }

        [When(@"I click the ""Delete"" button")]
        public void WhenIClickTheDeleteButton()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var deleteButton = wait.Until(driver =>
                driver.FindElement(By.XPath("//button[@data-testid='card-back-delete-card-button']"))
            );
            deleteButton.Click();
            Log.Information("Clicked the 'Delete' button.");
        }

        [When(@"I confirm the deletion")]
        public void WhenIConfirmTheDeletion()
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var confirmDeleteButton = wait.Until(driver =>
                driver.FindElement(By.XPath("//button[@data-testid='popover-confirm-button']"))
            );
            confirmDeleteButton.Click();
            Log.Information("Confirmed the deletion of the card.");
        }

        [Then(@"I should not see the card ""(.*)"" in the ""(.*)"" list")]
        public void ThenIShouldNotSeeTheCardInTheList(string cardName, string listName)
        {
            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            var cards = wait.Until(driver =>
                driver.FindElements(By.XPath($"//div[contains(@class, 'list') and contains(., '{listName}')]//div[contains(@class, 'list-card') and contains(., '{cardName}')]"))
            );
            Assert.IsFalse(cards.Count > 0, $"The card '{cardName}' was found in the '{listName}' list.");
            Log.Information("Verified that the card '{CardName}' is not present in the '{ListName}' list.", cardName, listName);
        }

        [AfterStep]
        public void AfterStep()
        {
            ScreenshotHelper.TakeScreenshot(_driver, ScenarioContext.Current.ScenarioInfo.Title);
        }

        [AfterScenario]
        public void TearDown()
        {
            _driver?.Quit(); 
            Log.Information("WebDriver instance has been quit.");
        }
    }
}
