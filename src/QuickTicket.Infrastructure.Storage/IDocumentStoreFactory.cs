using System.Threading.Tasks;

namespace QuickTicket.Infrastructure.Storage
{
    public interface IDocumentStoreFactory
    {
        Task<IDocumentStore> Create(string databaseName);
    }
}