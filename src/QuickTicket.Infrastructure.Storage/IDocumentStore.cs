namespace QuickTicket.Infrastructure.Storage
{
    public interface IDocumentStore
    {
        IDocumentSession<TDocument> CreateSession<TDocument>();
    }
}