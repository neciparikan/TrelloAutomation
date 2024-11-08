Feature: Trello Board API

  @create
  Scenario Outline: Create a new Trello board and a card on it
    Given I have a valid API key and token
    When I create a board with the name "<BoardName>"
    Then I should receive a response indicating the board was created
    And the board name should be "<BoardName>"
    When I retrieve the board ID for "<BoardName>"
    Then I should receive a response containing the board ID

    Examples:
      | BoardName           |
      | Test Board from API |

  @create
  Scenario Outline: Add a card to the To Do list on an existing Trello board
    Given I have a valid API key and token
    When I retrieve the board ID for "Test Board from API"
    Then I should receive a response containing the board ID
    When I create a card with the name "<CardName>" in the "To Do" list
    Then I should receive a response indicating the card was created
    And the card name should be "<CardName>"

    Examples:
      | CardName        |
      | Test API Card 1 |

  @read
  Scenario Outline: Create a new card and verify the read action
    Given I have a valid API key and token
    When I retrieve the board ID for "Test Board from API"
    Then I should receive a response containing the board ID
    When I create a card with the name "<CardName>" in the "To Do" list
    Then I should receive a response indicating the card was created
    And the card name should be "<CardName>"
    When I get the details of the card with the name "<CardName>"
    Then the details should match the expected values

    Examples:
      | CardName        |
      | Test API Card 2 |

  @update
  Scenario Outline: Update the status of a card
    Given I have a valid API key and token
    When I retrieve the board ID for "Test Board from API"
    Then I should receive a response containing the board ID
    When I create a card with the name "<CardName>" in the "To Do" list
    Then I should receive a response indicating the card was created
    And the card name should be "<CardName>"
    When I update the card status to "Doing"
    Then I should see the card "<CardName>" present in the "Doing" list (API)

    Examples:
      | CardName                 |
      | Test API Card for Update |

  @delete
  Scenario Outline: Delete a card from the board
    Given I have a valid API key and token
    When I retrieve the board ID for "Test Board from API"
    Then I should receive a response containing the board ID
    When I create a card with the name "<CardName>" in the "To Do" list
    Then I should receive a response indicating the card was created
    And the card name should be "<CardName>"
    When I delete the card with the name "<CardName>"
    Then I should receive a response indicating the card was deleted
    And the card "<CardName>" should not be present in the "To Do" list (API)

    Examples:
      | CardName                   |
      | Test API Card for Delete   |
