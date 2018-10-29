using InventoryAPI.Configuration;
using InventoryAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryAPI.Repository
{
    public class ItemsRepository : IItemsRepository
    {
        private IMongoCollection<Item> Collection { get; set; }

        public ItemsRepository(IOptions<MongoConnectionConfiguration> options)
        {
            // TODO: Research and add error handling
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.Database);
            Collection = database.GetCollection<Item>("items");
        }

        public async Task<IEnumerable<Item>> GetMany()
        {
            return await Collection.Find(Builders<Item>.Filter.Empty).ToListAsync();
        }

        public async Task<Item> GetOne(string id)
        {
            return await Collection.Find(i => i.Id == id).FirstOrDefaultAsync();
        }

        public async Task<Item> Add(Item item)
        {
            await Collection.InsertOneAsync(item);
            return item;
        }

        public async Task<bool> Remove(string id)
        {
            var actionResult = await Collection.DeleteOneAsync(i => i.Id == id);
            return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
        }

        public async Task<bool> Update(Item item)
        {
            var update = Builders<Item>.Update
                .Set(i => i.Name, item.Name)
                .Set(i => i.Model, item.Model)
                .Set(i => i.Category, item.Category)
                .Set(i => i.Cost, item.Cost)
                .Set(i => i.Notes, item.Notes)
                .Set(i => i.ReferenceTable, item.ReferenceTable);

            var actionResult = await Collection.UpdateOneAsync(i => i.Id == item.Id, update);
            return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }
    }
}