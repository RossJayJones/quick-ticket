using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus.Primitives;
using Microsoft.Extensions.Configuration;
using QuickTicket.Organisers.Commands;

namespace QuickTicket.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets<Program>()
                .Build();
            
            var bus = Bus.Factory.CreateUsingAzureServiceBus(configure =>
            {
                configure.SelectBasicTier();

                configure.Host(configuration["MassTransit:Url"], h =>
                {
                    h.SharedAccessSignature(s =>
                    {
                        s.KeyName = configuration["MassTransit:KeyName"];
                        s.SharedAccessKey = configuration["MassTransit:SharedAccessKey"];
                        s.TokenScope = TokenScope.Namespace;
                    });
                });
            });

            await bus.StartAsync();

            var endpoint = await bus.GetSendEndpoint(new Uri("queue:organisers"));

            while (true)
            {
                Console.WriteLine("Press enter to send a message");
                Console.ReadLine();
            
                await endpoint.Send<ICreateOrganiser>(new
                {
                    Name = "Test",
                    Description = "Test",
                    ContactNumber = "13213123",
                    EmailAddress = "213123",
                    WebsiteUrl = "31312"
                });    
            }
        }
    }
}