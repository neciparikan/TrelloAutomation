using BoDi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TechTalk.SpecFlow;
using TrelloAutomation.Config; 
using TrelloAutomation.Helpers; 
using TrelloAutomation.PageObjects;

namespace TrelloAutomation.Tests
{
    [Binding]
    public class Hooks
    {
        private readonly IObjectContainer _objectContainer;

        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;

            // Register services in the constructor
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            // Register TrelloSettings
            services.Configure<TrelloSettings>(configuration.GetSection("Trello"));
            services.AddHttpClient<ApiHelper>();
            services.AddSingleton<ILoginPage, LoginPage>();
            services.AddSingleton<IBoardPage, BoardPage>();

            var serviceProvider = services.BuildServiceProvider();

            // Register IOptions<TrelloSettings>
            var trelloSettings = serviceProvider.GetService<IOptions<TrelloSettings>>();
            _objectContainer.RegisterInstanceAs(trelloSettings);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            // Any additional setup before each scenario can be done here
        }
    }
}
