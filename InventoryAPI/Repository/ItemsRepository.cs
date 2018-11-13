using InventoryAPI.Configuration;
using InventoryAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryAPI.Repository
{
    public class ItemsRepository : IItemsRepository
    {
        private IMongoCollection<Item> Collection { get; set; }
        private IMongoCollection<BsonDocument> RawCollection { get; set; }

        public ItemsRepository(IOptions<MongoConnectionConfiguration> options)
        {
            // TODO: Research and add error handling
            var client = new MongoClient(options.Value.ConnectionString);
            var database = client.GetDatabase(options.Value.Database);
            Collection = database.GetCollection<Item>("items");
            RawCollection = database.GetCollection<BsonDocument>("items");
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

        public async Task<bool> UpdatePartial(string itemId, Item item)
        {
            var operations = BuildOperations(item);
            var updates = new List<UpdateDefinition<Item>>();

            foreach (var operation in operations)
            {
                if (operation.Value != null)
                {
                    dynamic value = Convert.ChangeType(operation.Value, operation.Type);
                    updates.Add(Builders<Item>.Update.Set(operation.Path, value));
                }
                else
                {
                    updates.Add(Builders<Item>.Update.Set(operation.Path, operation.Value));
                }
            }

            var partialUpdate = Builders<Item>.Update.Combine(updates);
            var actionResult = await Collection.UpdateOneAsync(i => i.Id == item.Id, partialUpdate);

            return actionResult.IsAcknowledged && actionResult.MatchedCount > 0;
        }

        private List<MongoOperation> BuildOperations(object obj, string parentName = null)
        {
            var operations = new List<MongoOperation>();

            var parentPath = parentName != null ? $"{parentName}." : "";
            var objType = obj.GetType();
            var objProperties = objType.GetProperties();
            var changed = objProperties.Where(p => p.Name == "ChangedProperties")?.FirstOrDefault();
            if (changed != null)
            {
                var changedObject = changed.GetValue(obj);
                var changedProperties = (IEnumerable<string>)changedObject;
                foreach(var changedProperty in changedProperties)
                {
                    var path = parentPath + changedProperty;
                    var propertyInfo = objType.GetProperty(changedProperty);
                    var value = propertyInfo.GetValue(obj);
                    operations.Add(new MongoOperation
                    {
                        Path = path,
                        Value = value,
                        Type = propertyInfo.PropertyType
                    });

                }
            }
            
            foreach (var property in objProperties)
            {
                var propValue = objType.GetProperty(property.Name).GetValue(obj);
                if (propValue != null)
                {
                    if (propValue.GetType().Namespace == "Castle.Proxies")
                    {
                        var path = parentPath + property.Name;
                        operations.AddRange(BuildOperations(propValue, property.Name));
                    }
                }
            }

            return operations;
        }
    }

    internal class MongoOperation
    {
        public string Path { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
    }
}