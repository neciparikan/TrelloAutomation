namespace TrelloAutomation.Models
{
    public class Board
    {
        public string Id { get; set; }  // Unique identifier for the board
        public string Name { get; set; } // Name of the board
    }

    public class Card
    {
        public string Id { get; set; }  // Unique identifier for the card
        public string Name { get; set; } // Name of the card
    }

    public class TrelloList
    {
        public string Id { get; set; }  // Unique identifier for the list
        public string Name { get; set; } // Name of the list
    }
}
