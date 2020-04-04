using System;
using System.Threading.Tasks;
using QuickTicket.Storage.CosmosDb.IntegrationTests.Fixtures;
using Xunit;

namespace QuickTicket.Storage.CosmosDb.IntegrationTests
{
    public class DocumentSessionTests
    {
        public class DescribeAdd
        {
            [Fact]
            public async Task WhenDocumentHasAlreadyBeenAdded_ItShouldThrowInvalidOperationException()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore();
                var session = documentStore.CreateSession();
                var document = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document);
                
                // Act
                var exception = Assert.Throws<InvalidOperationException>(() => session.Add(document));
                
                // Assert
                Assert.StartsWith("A document with id", exception.Message);
            }
        }

        public class DescribeLoadMany
        {
            [Fact]
            public async Task WhenDocumentIdsProvided_ItShouldLoadTheDocuments()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore();
                var session = documentStore.CreateSession();
                var document1 = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document1);
                var document2 = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document2);
                await session.SaveChanges();
                session = documentStore.CreateSession();
                
                // Act
                var documents = await session.LoadMany<TestDocument>(new[] {document1.Id, document2.Id});
                
                // Assert
                Assert.Contains(documents.Keys, id => document1.Id == id);
                Assert.NotNull(documents[document1.Id]);
                Assert.Contains(documents.Keys, id => document2.Id == id);
                Assert.NotNull(documents[document2.Id]);
            }

            [Fact]
            public async Task WhenDocumentNotFound_ItShouldReturnNull()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore();
                var session = documentStore.CreateSession();
                var documentId = Guid.NewGuid().ToString();
                
                // Act
                var documents = await session.LoadMany<TestDocument>(new[] { documentId });
                
                // Assert
                Assert.Contains(documents.Keys, id => documentId == id);
                Assert.Null(documents[documentId]);
            }
            
            [Fact]
            public async Task WhenDocumentHasNotBeenSavedToStorage_ItShouldBeRetrievable()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore();
                var session = documentStore.CreateSession();
                var document = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document);
                
                // Act
                var results = await session.LoadMany<TestDocument>(new []{ document.Id });

                // Assert
                Assert.Contains(results, pair => pair.Key == document.Id);
                Assert.Same(document, results[document.Id]);
            }
            
            [Fact]
            public async Task WhenDocumentMarkedForRemoval_ItShouldNotBeAbleToBeLoaded()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore();
                var session = documentStore.CreateSession();
                var document = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document);
                session.Remove(document);
                
                // Act
                var results = await session.LoadMany<TestDocument>(new []{ document.Id });

                // Assert
                Assert.Contains(results, pair => pair.Key == document.Id);
                Assert.Null(results[document.Id]);
            }
        }

        public class DescribeSaveChanges
        {
            [Fact]
            public async Task WhenEtagStale_ItShouldThrowException()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore();
                var session1 = documentStore.CreateSession();
                var document1 = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session1.Add(document1);
                await session1.SaveChanges();
                // Cause the etag to become stale
                var session2 = documentStore.CreateSession();
                await session2.LoadMany<TestDocument>(new[] {document1.Id});
                await session2.SaveChanges();
                
                // Act
                var exception = await Assert.ThrowsAsync<DocumentSessionException>(() => session1.SaveChanges());

                // Assert
                Assert.StartsWith("Received response status PreconditionFailed from Cosmos Db", exception.Message);
            }
        }
    }
}