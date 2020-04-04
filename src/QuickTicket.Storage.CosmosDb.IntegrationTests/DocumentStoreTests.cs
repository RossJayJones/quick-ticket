using System;
using System.Threading.Tasks;
using QuickTicket.Storage.CosmosDb.IntegrationTests.Fixtures;
using Xunit;

namespace QuickTicket.Storage.CosmosDb.IntegrationTests
{
    public class DocumentStoreTests
    {
        public class DescribeCreateSession
        {
            [Fact]
            public async Task WhenContainerInfoHasBeenSupplied_ItShouldReturnDocumentSession()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore();

                // Act
                var session = documentStore.CreateSession();
                
                // Assert
                Assert.NotNull(session);
            }
        }
    }
}