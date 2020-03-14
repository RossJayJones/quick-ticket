using System.Threading.Tasks;

namespace QuickTicket.Organisers.Domain
{
    public interface IOrganiserRepository
    {
        Task AddAsync(Organiser organiser);

        Task<Organiser> LoadAsync(OrganiserId organiserId);
    }
}