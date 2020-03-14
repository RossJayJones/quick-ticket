using System.Threading.Tasks;
using QuickTicket.Organisers.Domain;

namespace QuickTicket.Organisers.Infrastructure
{
    public class OrganiserRepository : IOrganiserRepository
    {
        public Task AddAsync(Organiser organiser)
        {
            return Task.CompletedTask;
        }

        public Task<Organiser> LoadAsync(OrganiserId organiserId)
        {
            return Task.FromResult<Organiser>(null);
        }
    }
}