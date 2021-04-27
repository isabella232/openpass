using Criteo.AspNetCore.Helpers;
using Microsoft.AspNetCore.Hosting;

namespace OpenPass.IdController
{
    public class Program
    {
        /// <summary>
        /// Initialization of the ASP.NET Core framework.
        /// </summary>
        public static void Main(string[] args)
        {
            // Hosts a Criteo AspNetCore WebApi service, exposing REST business apis (your controllers) and administrative handlers on 2 different servers/ports
            var builder = WebHostBuilderHelper
                .CreateHybridCriteoBuilder(args);

            /* If you don't want to register anything in the ServiceCollection, use CreatePureCriteoBuilder(args).
             * Check https://confluence.criteois.com/display/WS/Services%2C+dependencies+and+providers for more information. */

            ConfigureApp(builder);

            builder.Build().Run(); // Launches initialization and blocks.
        }

        /// <summary>
        /// App-specific AspNetCore WebHost customization goes here
        /// eg.: builder.ConfigureServices( services => ... )
        /// </summary>
        /// <remarks>This public method can be used to initialize the app in tests "in isolation".</remarks>
        public static void ConfigureApp(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>(); // Startup class contains initialization code (services, specific routes)
        }
    }
}
