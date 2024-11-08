using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Serilog;
using TrelloAutomation.Config;
using Microsoft.Extensions.Options;
using System;

namespace TrelloAutomation.PageObjects
{
    public class LoginPage : ILoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;
        private readonly string _trelloUrl;

        private readonly By _emailField = By.Id("username");
        private readonly By _passwordField = By.Id("password");
        private readonly By _loginButton = By.Id("login-submit");
        private readonly By _errorMessage = By.XPath("//section[@data-testid='form-error']");

       
        public LoginPage(IWebDriver driver, IOptions<TrelloSettings> settings)
        {
            _driver = driver ?? throw new ArgumentNullException(nameof(driver));
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));
            _trelloUrl = settings.Value.Url;
        }

        public void EnterEmail(string email)
        {
            Log.Information("Entering email: {Email}", email);
            _wait.Until(driver => driver.FindElement(_emailField).Displayed);
            _driver.FindElement(_emailField).SendKeys(email);
        }

        public void EnterPassword(string password)
        {
            Log.Information("Entering password.");
            _wait.Until(driver => driver.FindElement(_passwordField).Displayed);
            _driver.FindElement(_passwordField).SendKeys(password);
        }

        public void ClickLogin()
        {
            Log.Information("Clicking login button.");
            _driver.FindElement(_loginButton).Click();
        }

        public string GetErrorMessage()
        {
            try
            {
                var errorMsgElement = _driver.FindElement(_errorMessage);
                return errorMsgElement.Displayed ? errorMsgElement.Text : "Error message not found.";
            }
            catch (NoSuchElementException)
            {
                return "Error message element not found.";
            }
        }

        public void NavigateToLoginPage()
        {
            Log.Information("Navigating to login page.");
            _driver.Navigate().GoToUrl(_trelloUrl);
        }

        public void ClickButton(string buttonText)
        {
            if (buttonText.Equals("Log in", StringComparison.OrdinalIgnoreCase))
            {
                _driver.FindElement(By.LinkText(buttonText)).Click();
            }
            else if (buttonText.Equals("Continue", StringComparison.OrdinalIgnoreCase))
            {
                _driver.FindElement(_loginButton).Click();
            }
        }

        public void WaitForElementToBeVisible(By locator)
        {
            _wait.Until(driver => driver.FindElement(locator).Displayed);
        }

        public void ContinueWithoutTwoStepVerification()
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
    }
}
