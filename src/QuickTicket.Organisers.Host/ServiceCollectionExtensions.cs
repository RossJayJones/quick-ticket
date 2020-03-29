using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuickTicket.Organisers.Domain;
using QuickTicket.Organisers.Domain.Infrastructure;
using QuickTicket.Organisers.Host.Consumers;

namespace QuickTicket.Organisers.Host
{
    public static class ServiceCollectionExtensions
    {
        public static void AddOrganisersApplicationServices(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<IOrganiserRepository, OrganiserRepository>();
            
            // Register Consumers
            services.AddScoped<CreateOrganiserConsumer>();
        }

        public static void AddOrganisersBus(this IServiceCollection services,
            MassTransitConfiguration configuration)
        {
            services.AddMassTransit(configurator =>
            {
                configurator.AddBus(provider => Bus.Factory.CreateUsingAzureServiceBus(configure =>
                { 
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
                        c.Consumer<CreateOrganiserConsumer>(provider);
                    });
                }));

                //x.AddRequestClient<SubmitOrder>();
            });

            services.AddSingleton<IHostedService, BusService>();
        }
    }
}