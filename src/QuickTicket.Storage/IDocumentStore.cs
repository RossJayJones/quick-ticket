using System.Threading.Tasks;

namespace QuickTicket.Storage
{
    public interface IDocumentStore
    {
        Task Init();

        IDocumentSession CreateSession();
    }
}