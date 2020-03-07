using System.Threading.Tasks;
using MassTransit;
using QuickTicket.Organisers.Commands;
using Serilog;

namespace QuickTicket.Organisers.Host.Consumers
{
    public class CreateOrganiserConsumer : IConsumer<ICreateOrganiser>
    {
        public Task Consume(ConsumeContext<ICreateOrganiser> context)
        {
            Log.Information($"Create {context.Message.Name}");
            
            return Task.CompletedTask;
        }
    }
}