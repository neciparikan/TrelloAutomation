using BoDi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using TrelloAutomation.Helpers;
using TrelloAutomation.PageObjects;
using TrelloAutomation.Config;

namespace TrelloAutomation
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration; 
        }

        public void ConfigureServices(IServiceCollection services)
        {
            
            var trelloSettings = new TrelloSettings();
            Configuration.Bind("Trello", trelloSettings);

            services.Configure<TrelloSettings>(Configuration.GetSection("Trello"));
            services.AddHttpClient<ApiHelper>();
            services.AddSingleton<ILoginPage, LoginPage>();
            services.AddSingleton<IBoardPage, BoardPage>();
        }
    }
}