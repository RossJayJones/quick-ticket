using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace QuickTicket.Organisers.Host
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureLogging((context, loggingBuilder) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .ReadFrom.Configuration(context.Configuration)
                        .CreateLogger();
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddOrganisersApplicationServices();
                    
                    services.AddOrganisersBus(context.Configuration
                        .GetSection(MassTransitConfiguration.SectionName)
                        .Get<MassTransitConfiguration>());
                });

            await host.RunConsoleAsync();
        }
    }
}