namespace QuickTicket.Storage.CosmosDb
{
    public class CosmosDbConfiguration
    {
        public static readonly string SectionName = "CosmosDb";
        
        public string ConnectionString { get; set; }

        public string DatabaseName { get; set; }
    }
}