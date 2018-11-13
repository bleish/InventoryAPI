using ChangeTracking;
using InventoryAPI.Configuration;
using InventoryAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            // var trackable = item.CastToIChangeTrackable();
            // var fetalTrackable = item.Fetal.CastToIChangeTrackable();

            // List<UpdateDefinition<Item>> updates = new List<UpdateDefinition<Item>>();
            // // var update = Builders<Item>.Update;
            // // var update = new UpdateDefinition<Item>();

            // foreach (var property in trackable.ChangedProperties)
            // {
            //     // var prop = item.GetType().GetProperty(property);
            //     // var type = prop.PropertyType;
            //     // if (type.Assembly.GetName().Name == "InventoryAPI")
            //     // {
            //     //     var stuff = prop.PropertyType.GetProperties();
            //     // }
            //     // var value = prop.GetValue(item);
            //     // dynamic bland = Convert.ChangeType(value, type);
            //     updates.Add(Builders<Item>.Update.Set(property, "hi"));
            // }

            // var combination = Builders<Item>.Update;

            // UpdateDefinition<Item> update = combination.Combine(updates);

            // var actionResult = await Collection.UpdateOneAsync(i => i.Id == item.Id, update);

            // return actionResult.IsAcknowledged && actionResult.MatchedCount > 0;

            // var update = Builders<Item>.Update;
            // foreach (var operation in changeDoc.Operations)
            // {
            //     if (operation.Operation == ItemUpdateOperationType.Replace)
            //     {
            //         update.Set(i => i.Notes = )
            //         var bleh = typeof(Item).GetProperty(operation.Path);
            //         var bleh2 = Convert.ChangeType(operation.Value, bleh.PropertyType);
            //         update.Set()
            //     }
            // }

            // TODO: Delete this
            // if (changeDoc.ElementCount > 0)
            // {
            //     var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(itemId));
            //     UpdateDefinition<BsonDocument> update = changeDoc;
            //     var actionResult = await RawCollection.UpdateOneAsync(filter, update);
            //     return actionResult.IsAcknowledged && actionResult.MatchedCount > 0;
            // }
            // return false;

            // TODO: Delete this
            // var itemObjId = new ObjectId(itemId);
            // var itemBson = item.ToBsonDocument();
            // if (itemBson.ElementCount > 0)
            // {
            //     var filter = Builders<BsonDocument>.Filter.Eq("_id", itemObjId);
            //     var update = new BsonDocumentUpdateDefinition<BsonDocument>(new BsonDocument("$set", itemBson));
            //     var actionResult = await RawCollection.UpdateOneAsync(filter, update);
            //     return actionResult.IsAcknowledged && actionResult.MatchedCount > 0;
            // }
            // return false;

            // TODO: Delete this
            // var update = Builders<Item>.Update.Set(item);
            // var actionResult = await Collection.UpdateOneAsync(i => i.Id == itemId, update);
            
            // var partialItem = BsonDocument.Parse(changes);
            // var filter = Builders<BsonDocument>.Filter.Eq("_id", itemId);
            // UpdateDefinition<BsonDocument> update = new BsonDocumentUpdateDefinition<BsonDocument>(new BsonDocument("$set", partialItem));
            // var actionResult = await BsonCollection.UpdateOneAsync(filter, update);

            // return actionResult.IsAcknowledged && actionResult.MatchedCount > 0;
        }
    }

    internal class MongoOperation
    {
        public string Path { get; set; }
        public object Value { get; set; }
        public Type Type { get; set; }
    }
}