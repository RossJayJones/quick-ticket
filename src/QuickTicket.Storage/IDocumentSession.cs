using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickTicket.Storage
{
    public interface IDocumentSession
    {
        /// <summary>
        /// When implemented by a derived class the document will be added to the current session
        /// </summary>
        void Add<TDocument>(TDocument document);

        /// <summary>
        /// When implemented by a derived class the document will be updated
        /// </summary>
        void Update<TDocument>(TDocument document);

        /// <summary>
        /// When implemented by a derived class the document will be marked for removal
        /// </summary>
        void Remove<TDocument>(TDocument document);
        
        /// <summary>
        /// When implemented by a derived class one or more documents are retrieved for the give collection of document ids
        /// </summary>
        Task<IReadOnlyDictionary<string, TDocument>> LoadMany<TDocument>(IEnumerable<string> documentIds);

        /// <summary>
        /// When implemented by a derived class changes to the documents loaded into the session will be persisted
        /// to the underlying storage.
        /// </summary>
        /// <returns></returns>
        Task SaveChanges();
    }
}