namespace QuickTicket.Storage
{
    public interface IDocumentStore
    {
        IDocumentSession<TDocument> CreateSession<TDocument>()
            where TDocument : class;
    }
}