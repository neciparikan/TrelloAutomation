using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace TrelloAutomation.Helpers
{
    public static class WebDriverFactory
    {
        public static IWebDriver CreateDriver()
        {
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized"); // Start the browser in a maximized window
            options.AddArgument("--disable-infobars"); // Disable infobars that appear on the top of the Chrome window

            return new ChromeDriver(options); // Instantiate and return a new ChromeDriver with the specified options
        }
    }
}
