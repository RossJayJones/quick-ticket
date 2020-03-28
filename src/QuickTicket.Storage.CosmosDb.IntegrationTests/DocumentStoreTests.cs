using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using QuickTicket.Storage.CosmosDb.IntegrationTests.Fixtures;
using Xunit;

namespace QuickTicket.Storage.CosmosDb.IntegrationTests
{
    public class DocumentStoreTests
    {
        public class DescribeCreateSession
        {
            [Fact]
            public async Task WhenContainerInfoHasNotBeenSupplied_ItShouldThrowInvalidOperationException()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore();

                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => documentStore.CreateSession<TestDocument>());
                
                // Assert
                Assert.StartsWith("A container has not been registered for document type", exception.Message);
            }

            [Fact]
            public async Task WhenContainerInfoHasBeenSupplied_ItShouldReturnDocumentSession()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore(new Dictionary<Type, ContainerInfo>
                {
                    [typeof(TestDocument)] = FixtureHelper.CreateTestContainerInfo()
                });

                // Act
                var session = documentStore.CreateSession<TestDocument>();
                
                // Assert
                Assert.NotNull(session);
            }
        }
    }
}