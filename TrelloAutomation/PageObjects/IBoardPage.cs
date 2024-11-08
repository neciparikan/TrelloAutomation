using OpenQA.Selenium;

namespace TrelloAutomation.PageObjects
{
    public interface IBoardPage
    {
        void ClickCreateBoardButton(); // Click the button to create a new board

        void EnterBoardName(string boardName); // Enter the name for the new board

        void ClickCreateButton(); // Click the button to create the board

        bool IsBoardDisplayed(string boardName); // Check if a specific board is displayed on the page

        bool IsErrorMessageDisplayed(); // Check if an error message is displayed when creating a board

        void ClickAddCardButton(); // Click "Add a card" in the "To Do" section

        void EnterCardName(string cardName); // Enter the name of a new card

        void ClickSaveButton(); // Click the "Add card" button to save the new card

        bool IsCardDisplayed(string cardName); // Check if a specific card is displayed

        bool IsCardDisplayedInList(string cardName, string listName); // Check if a card is displayed in a specific list
    }
}
