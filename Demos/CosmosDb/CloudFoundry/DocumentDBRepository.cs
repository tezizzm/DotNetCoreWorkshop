using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CloudFoundry.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace CloudFoundry
{
    public class DocumentDbRepository : IDocumentDbRepository
    {
        private string CollectionId { get; } = "Items";
        private readonly DocumentClient _client;
        private readonly CosmosDbInfo _cosmosDbInfo;

        public DocumentDbRepository(CosmosDbInfo cosmosDbInfo)
        {
            _cosmosDbInfo = cosmosDbInfo;
            _client = new DocumentClient(new Uri(cosmosDbInfo.Endpoint), cosmosDbInfo.Key);
            CreateDatabaseIfNotExistsAsync().Wait();
            CreateCollectionIfNotExistsAsync().Wait();
        }

        public async Task<Item> GetItemAsync(string id)
        {
            try
            {
                Document document = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_cosmosDbInfo.DatabaseId, CollectionId, id));
                return (Item)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(Expression<Func<Item, bool>> predicate)
        {
            IDocumentQuery<Item> query = _client.CreateDocumentQuery<Item>(
                UriFactory.CreateDocumentCollectionUri(_cosmosDbInfo.DatabaseId, CollectionId),
                new FeedOptions { MaxItemCount = -1 })
                .Where(predicate)
                .AsDocumentQuery();

            List<Item> results = new List<Item>();
            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<Item>());
            }

            return results;
        }

        public async Task<Document> CreateItemAsync(Item item)
        {
            return await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_cosmosDbInfo.DatabaseId, CollectionId), item);
        }

        public async Task<Document> UpdateItemAsync(string id, Item item)
        {
            return await _client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(_cosmosDbInfo.DatabaseId, CollectionId, id), item);
        }

        public async Task DeleteItemAsync(string id)
        {
            await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_cosmosDbInfo.DatabaseId, CollectionId, id));
        }

        private async Task CreateDatabaseIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(_cosmosDbInfo.DatabaseId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = _cosmosDbInfo.DatabaseId });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateCollectionIfNotExistsAsync()
        {
            try
            {
                await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(_cosmosDbInfo.DatabaseId, CollectionId));
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(_cosmosDbInfo.DatabaseId),
                        new DocumentCollection { Id = CollectionId },
                        new RequestOptions { OfferThroughput = 1000 });
                }
                else
                {
                    throw;
                }
            }
        }
    }
}