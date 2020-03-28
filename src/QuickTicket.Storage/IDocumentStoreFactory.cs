using System.Threading.Tasks;

namespace QuickTicket.Storage
{
    public interface IDocumentStoreFactory
    {
        Task<IDocumentStore> Create(string databaseName);
    }
}