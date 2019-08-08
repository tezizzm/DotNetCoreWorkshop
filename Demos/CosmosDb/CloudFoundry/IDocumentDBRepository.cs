using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CloudFoundry.Models;
using Microsoft.Azure.Documents;

namespace CloudFoundry
{
    public interface IDocumentDbRepository
    {
        Task<Document> CreateItemAsync(Item item);
        Task DeleteItemAsync(string id);
        Task<Item> GetItemAsync(string id);
        Task<IEnumerable<Item>> GetItemsAsync(Expression<Func<Item, bool>> predicate);
        Task<Document> UpdateItemAsync(string id, Item item);
    }
}