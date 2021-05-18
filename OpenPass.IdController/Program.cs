using App.Metrics.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace OpenPass.IdController
{
    public class Program
    {
        /// <summary>
        /// Initialization of the ASP.NET Core framework.
        /// </summary>
        public static void Main(string[] args)
        {
            var builder = CreateWebHostBuilder(args);

            ConfigureApp(builder);

            builder.Build().Run(); // Launches initialization and blocks.
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseMetrics()
            .ConfigureLogging((hostingContext, logging) =>
            {
                logging.ClearProviders();
                logging.AddConsole();
            })
            .UseStartup<Startup>();

        public static void ConfigureApp(IWebHostBuilder builder)
        {
            builder.UseStartup<Startup>(); // Startup class contains initialization code (services, specific routes)
        }
    }
}
