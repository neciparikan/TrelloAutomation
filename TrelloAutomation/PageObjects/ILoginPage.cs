using OpenQA.Selenium;

namespace TrelloAutomation.PageObjects
{
    public interface ILoginPage
    {
        void EnterEmail(string email); // Enter the user's email into the email field

        void EnterPassword(string password); // Enter the user's password into the password field

        void ClickLogin(); // Click the login button to submit the login form

        string GetErrorMessage(); // Retrieve the error message displayed on the login page

        void NavigateToLoginPage(); // Navigate to the Trello login page

        void ClickButton(string buttonText); // Click a button based on the button's text

        void ContinueWithoutTwoStepVerification(); // Continue without two-step verification if the button is visible

        void WaitForElementToBeVisible(By locator); // Wait for a specific element to be visible on the page
    }
}
