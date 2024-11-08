Feature: Login Functionality

  Scenario: Verify Home Page Title
    Given I navigate to the Trello home page
    Then I should see the title "Manage Your Team’s Projects From Anywhere"

  Scenario: Login with valid credentials
    Given I navigate to the Trello home page
    When I click the "Log in" button
    And I enter my email
    And I click the "Continue" button
    And I enter my password
    And I submit login
    And I continue without two-step verification if the button is visible
    Then I should be redirected to the Trello dashboard

  Scenario: Login with invalid credentials
    Given I navigate to the Trello home page
    When I click the "Log in" button
    And I enter my email
    And I click the "Continue" button
    And I enter wrong password
    And I submit login
    Then I should see an error message containing "Incorrect email address and / or password"
