using System;
using System.Runtime.Serialization;

namespace QuickTicket.Storage.CosmosDb
{
    [Serializable]
    public class DocumentSessionException : Exception
    {
        public DocumentSessionException(string message, Exception innerException = null)
            : base(message, innerException)
        {
            
        }

        public DocumentSessionException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
            
        }
    }
}