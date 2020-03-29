using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
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
                var documentStore = await FixtureHelper.CreateDocumentStore(new Dictionary<Type, ContainerInfo>
                {
                    [typeof(TestDocument)] = FixtureHelper.CreateTestContainerInfo()
                });
                var session = documentStore.CreateSession<TestDocument>();
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
                var documentStore = await FixtureHelper.CreateDocumentStore(new Dictionary<Type, ContainerInfo>
                {
                    [typeof(TestDocument)] = FixtureHelper.CreateTestContainerInfo()
                });
                var session = documentStore.CreateSession<TestDocument>();
                var document1 = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document1);
                var document2 = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document2);
                await session.SaveChanges();
                session = documentStore.CreateSession<TestDocument>();
                
                // Act
                var documents = await session.LoadMany(new[] {document1.Id, document2.Id});
                
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
                var documentStore = await FixtureHelper.CreateDocumentStore(new Dictionary<Type, ContainerInfo>
                {
                    [typeof(TestDocument)] = FixtureHelper.CreateTestContainerInfo()
                });
                var session = documentStore.CreateSession<TestDocument>();
                var documentId = Guid.NewGuid().ToString();
                
                // Act
                var documents = await session.LoadMany(new[] { documentId });
                
                // Assert
                Assert.Contains(documents.Keys, id => documentId == id);
                Assert.Null(documents[documentId]);
            }
            
            [Fact]
            public async Task WhenDocumentHasNotBeenSavedToStorage_ItShouldBeRetrievable()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore(new Dictionary<Type, ContainerInfo>
                {
                    [typeof(TestDocument)] = FixtureHelper.CreateTestContainerInfo()
                });
                var session = documentStore.CreateSession<TestDocument>();
                var document = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document);
                
                // Act
                var results = await session.LoadMany(new []{ document.Id });

                // Assert
                Assert.Contains(results, pair => pair.Key == document.Id);
                Assert.Same(document, results[document.Id]);
            }
            
            [Fact]
            public async Task WhenDocumentMarkedForRemoval_ItShouldNotBeAbleToBeLoaded()
            {
                // Arrange
                var documentStore = await FixtureHelper.CreateDocumentStore(new Dictionary<Type, ContainerInfo>
                {
                    [typeof(TestDocument)] = FixtureHelper.CreateTestContainerInfo()
                });
                var session = documentStore.CreateSession<TestDocument>();
                var document = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session.Add(document);
                session.Remove(document);
                
                // Act
                var results = await session.LoadMany(new []{ document.Id });

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
                var documentStore = await FixtureHelper.CreateDocumentStore(new Dictionary<Type, ContainerInfo>
                {
                    [typeof(TestDocument)] = FixtureHelper.CreateTestContainerInfo()
                });
                var session1 = documentStore.CreateSession<TestDocument>();
                var document1 = new TestDocument {Id = Guid.NewGuid().ToString(), PartitionKey = "Tests"};
                session1.Add(document1);
                await session1.SaveChanges();
                // Cause the etag to become stale
                var session2 = documentStore.CreateSession<TestDocument>();
                await session2.LoadMany(new[] {document1.Id});
                await session2.SaveChanges();
                
                // Act
                var exception = await Assert.ThrowsAsync<DocumentSessionException>(() => session1.SaveChanges());

                // Assert
                Assert.StartsWith("Received response status PreconditionFailed from Cosmos Db", exception.Message);
            }
        }
    }
}