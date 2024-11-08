using System;
using System.IO;
using OpenQA.Selenium;
using Serilog;

namespace TrelloAutomation.Helpers
{
    public static class ScreenshotHelper
    {
        public static void TakeScreenshot(IWebDriver driver, string testName)
        {
            var screenshot = ((ITakesScreenshot)driver).GetScreenshot();

            // Create a folder structure based on the current date and test name
            string dateFolder = DateTime.Now.ToString("yyyyMMdd");
            string baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots", dateFolder, testName);
            
            // Create the directory if it doesn't exist
            Directory.CreateDirectory(baseDirectory);

            // Create a file name for the screenshot with a timestamp for uniqueness
            var fileName = $"{DateTime.Now:HHmmss}.png";

            // Define the file path where the screenshot will be saved
            var filePath = Path.Combine(baseDirectory, fileName);

            // Save the screenshot
            screenshot.SaveAsFile(filePath);
            Log.Information($"Screenshot saved: {filePath}");
        }
    }
}
