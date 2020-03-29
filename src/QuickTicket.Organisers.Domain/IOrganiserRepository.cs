using System.Threading.Tasks;

namespace QuickTicket.Organisers.Domain
{
    public interface IOrganiserRepository
    {
        void Add(Organiser organiser);

        void Update(Organiser organiser);

        Task<Organiser> LoadAsync(OrganiserId organiserId);
    }
}