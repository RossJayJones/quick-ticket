using System.Threading.Tasks;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickTicket.Organisers.Host.Consumers;
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
                    var configuration = context.Configuration
                        .GetSection(MassTransitConfiguration.SectionName)
                        .Get<MassTransitConfiguration>();
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(configure =>
                        { 
                            configure.SelectBasicTier();
                            
                            configure.Host(configuration.Url, h =>
                            {
                                h.SharedAccessSignature(s =>
                                {
                                    s.KeyName = configuration.KeyName;
                                    s.SharedAccessKey = configuration.SharedAccessKey;
                                    s.TokenScope = TokenScope.Namespace;
                                });
                            });
                            
                            configure.ReceiveEndpoint("organisers", c =>
                            {
                                c.SelectBasicTier();
                                c.Consumer<CreateOrganiserConsumer>();
                            });
                        }));

                        //x.AddRequestClient<SubmitOrder>();
                    });

                    services.AddSingleton<IHostedService, BusService>();
                });

            await host.RunConsoleAsync();
        }
    }
}