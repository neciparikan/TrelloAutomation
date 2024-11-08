# Trello Automation Project

## Overview

The **Trello Automation Project** demonstrates the implementation of CRUD operations for boards and cards in Trello, utilizing automation testing strategies with C#. This project is designed to ensure the quality and reliability of the Trello application through comprehensive UI and API testing.

In this project, It is utilized DI to manage configurations through `TrelloSettings`, making it easier to access and manage configuration values throughout the application.

## Preconditions

Before running the tests, you need to acquire your API key and token from Trello:

- **API Key:** Obtain your API key from the following link:
Trello Power-Ups

- **Token Generation:** Generate a new token using your API key from the link below:

https://trello.com/1/authorize?expiration=1day&name=Knab&scope=read,write&response_type=token&key=<your-api-key>

### Credentials

You must add your credentials to the `appsettings.json` file before running the tests. The structure should resemble the following:

"Credentials": {
    "ValidEmail": "<your-email>",
    "ValidPassword": "<your-valid-password>",
    "InvalidPassword": "<your-invalid-password>",
    "ApiKey": "<your-api-key>", 
    "ApiToken": "<your-api-token>"
}

## How It Works

**IMPORTANT NOTICE:** The token and key must be added to the `appsettings.json` file before starting the automation.

## Test Design Dependencies

- **UI Tests:** The board for UI testing (named "Test") is created only once and must have default lists like "To Do," "Doing," and "Done." Ensure that this board does not already exist before running the UI tests. The tests will handle the creation, reading, updating, and deletion operations sequentially.

- **API Tests:** The board for API testing (named "Test Board from API") is also created only once. Confirm that this board does not exist before executing the API tests. The API tests will perform create, read, update, and delete operations after the board is established.

### Pre-Test Requirement

After logging into the workspace, verify that there is no "View Closed Boards" option available on the main page. This ensures that any closed boards do not interfere with the current testing environment.

**Important:** The order of test execution can affect results. The `dotnet test` command may not guarantee the sequence of operations. Therefore, if the tests are run in a manner that changes the order of the `Read`, `Update`, and `Delete` operations, it may lead to failures. It is crucial to manage the dependencies carefully to maintain the integrity of the tests.

### Setup Requirements

1. **Install .NET SDK:** Ensure you have the .NET SDK (version 8.0 or higher) installed on your machine.
2. **Install Visual Studio Code:** Download and install Visual Studio Code.
3. **C# Extension:** Install the C# extension for VS Code for better development support.
4. **ChromeDriver:** Make sure the ChromeDriver executable is available in your system's path.

### Building the Project

1. **Clone the Repository:**
git clone https://github.com/yourusername/TrelloAutomation.git
cd TrelloAutomation

2. **Open the Project in Visual Studio Code:**
   - Launch Visual Studio Code and open the cloned repository folder.

3. **Restore NuGet Packages:**
   - Open the integrated terminal in VS Code (``Ctrl + ` ``) and run:
   dotnet restore

   
4. **Build the Project:**
   - To build the project, run the following command in the terminal:
   dotnet build
   
### Running the Tests

1. **Execute Tests with Logging:**
   - You can run all tests using the following command in the terminal and log the results to a `.trx` file:
   dotnet test --logger "trx;LogFileName=TestResult.trx"

2. **View Results:**
   - Test results will be available in the terminal output and will also be saved in the `TestResult.trx` file located in the test project directory.

## Screenshots

Screenshots taken during the tests are saved in the following directory:

`{ProjectDirectory}\TrelloAutomation\bin\Debug\net8.0\Screenshots`

Where `{ProjectDirectory}` is the root directory of the cloned repository. The screenshots provide visual verification of the test execution at various stages.

## Project Structure

The project includes below structure to organize the tests:

# TRELLOAUTOMATION Project Structure

<details>
<summary>Expand to view project structure</summary>

TRELLOAUTOMATION
── TrelloAutomation/
 ├── Config/
 └── TrelloSettings.cs // Configuration settings for Trello
── Features/
 ├── Login.feature // Gherkin feature file for login scenarios
 ├── BoardManagement.feature // Gherkin feature file for board management scenarios
 └── BoardApiTests.feature // Gherkin feature file for API tests related to board and card creation
── Helpers/
 ├── WebDriverFactory.cs // Helper class for creating WebDriver instances
 ├── ScreenshotHelper.cs // Helper class for taking screenshots
 └── ApiHelper.cs // Helper class for API requests
── Models/
 └── TrelloModels.cs // Model class for Trello List
── PageObjects/
 ├── IBoardPage.cs // Interface for board page interactions
 ├── BoardPage.cs // Implementation of IBoardPage
 ├── ILoginPage.cs // Interface for login page interactions
 └── LoginPage.cs // Implementation of ILoginPage
── Startup.cs // Startup class for configuring services
│ ├── Tests/
│ │ ├── APITests/ // Folder for API test cases
│ │ │ ├── TrelloApiTests.cs // API test cases for Trello
│ │ │ └── BoardApiSteps.cs // Step definitions for Board API tests
│ │ └── UITests/ // Folder for UI test cases
│ │ ├── BoardSteps.cs // Step definitions for BoardManagement scenarios
│ │ └── Hooks.cs // Hooks class for SpecFlow
│ ├── TestResults/
│ │ └── TestResult.trx // Test result file
│ ├── appsettings.json // Configuration settings file
│ ├── LivingDoc.html // Living documentation file
│ ├── specflow.json // SpecFlow configuration file
│ ├── Dockerfile // Dockerfile for the project
│ ├── docker-compose.yml // Docker Compose configuration
│ └── Test Plan/ // Folder containing the Test Plan document
│ └── Test Plan for Trello Automation Project.pdf
├── TrelloAutomation.sln // Solution file for managing the project
└── README.md // This README file

</details>



## Requirements of the Test

- Users should be able to create, edit, and delete a board and a card.
- All CRUD operations must be tested thoroughly with positive, negative, and edge cases.

## Additional Information

- For documentation related to SpecFlow, please refer to the `LivingDoc.html` file.
- If you encounter any issues, please check the logs generated by Serilog in the console output for debugging information.
