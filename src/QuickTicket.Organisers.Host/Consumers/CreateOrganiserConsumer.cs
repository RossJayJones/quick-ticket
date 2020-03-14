using System.Threading.Tasks;
using MassTransit;
using QuickTicket.Core;
using QuickTicket.Organisers.Commands;
using QuickTicket.Organisers.Domain;

namespace QuickTicket.Organisers.Host.Consumers
{
    public class CreateOrganiserConsumer : IConsumer<ICreateOrganiser>
    {
        private readonly IOrganiserRepository _repository;

        public CreateOrganiserConsumer(IOrganiserRepository repository)
        {
            _repository = repository;
        }
        
        public async Task Consume(ConsumeContext<ICreateOrganiser> context)
        {
            var organiser = Organiser.Create(
                context.Message.Name,
                context.Message.Description,
                new ContactNumber(context.Message.ContactNumber),
                new EmailAddress(context.Message.EmailAddress),
                new WebsiteUrl(context.Message.WebsiteUrl),
                null,
                null);
            await _repository.AddAsync(organiser);
        }
    }
}