Feature: Board Management Functionality

  @create
  Scenario Outline: Create a new Trello board and add a card
    Given I am logged into Trello
    Then I should be redirected to the Trello dashboard
    When I click on the "Create new board" button
    And I enter a valid board name "<BoardName>"
    And I click the "Create Board" button
    Then I should see the newly created board in my list of boards
    And I should see the title "<BoardTitle>"
    When I click the "Add a card" button
    And I enter the card name "<CardName>"
    And I click the "Save" button to create the card
    Then I should see the card "<CardName>" in the list of cards

    Examples:
      | BoardName | BoardTitle | CardName         |
      | Test      | Test       | TrelloAutomation |

  @read
  Scenario Outline: Read the Trello card details
    Given I am logged into Trello
    When I click on the "<BoardName>" board
    And I click the "Add a card" button
    And I enter the card name "<CardName>"
    And I click the "Save" button to create the card
    And I click the card input field for "<CardName>"
    Then I should see the information containing "necip added this card to To Do"

    Examples:
      | BoardName | CardName  |
      | Test      | TestCard1 |

  @update
  Scenario Outline: Update the card status
    Given I am logged into Trello
    When I click on the "<BoardName>" board
    And I click the "Add a card" button
    And I enter the card name "<CardName>"
    And I click the "Save" button to create the card
    And I click the card input field for "<CardName>"
    When I click the "To Do" dropdown button
    And I select "Doing"
    And I click the "Move" button
    When I close the card details window
    Then I should see the card "<CardName>" in the "Doing" list

    Examples:
      | BoardName | CardName  |
      | Test      | TestCard2 |

  @delete
  Scenario Outline: Delete the card
    Given I am logged into Trello
    When I click on the "<BoardName>" board
    And I click the "Add a card" button
    And I enter the card name "<CardName>"
    And I click the "Save" button to create the card
    And I click the card input field for "<CardName>"
    When I click the "Archive" button
    And I click the "Delete" button
    And I confirm the deletion
    Then I should not see the card "<CardName>" in the "To Do" list

    Examples:
      | BoardName | CardName  |
      | Test      | TestCard3 |
