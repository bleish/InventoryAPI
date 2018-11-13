using System;
using System.Collections.Generic;
using MongoDB.Bson;

namespace InventoryAPI.Models
{
    public class ItemUpdate
    {
        // public Dictionary<string, object> Replace { get; set; }
        // public Dictionary<string, object> Remove { get; set; }

        public List<ItemUpdateOperation> Operations { get; set; }

        public BsonDocument ToBsonDocument()
        {
            var changeDoc = new BsonDocument();
            foreach (var operation in Operations)
            {
                var fieldChange = new BsonDocument(new Dictionary<string, object> { [operation.Path] = operation.Value });
                switch (operation.Operation)
                {
                    case ItemUpdateOperationType.Replace:
                        changeDoc["$set"] = fieldChange;
                        break;
                    case ItemUpdateOperationType.Remove:
                        changeDoc["$unset"] = fieldChange;
                        break;
                    default:
                        throw new Exception();
                }
            }

            return changeDoc;

            // changeDoc["$set"] = new BsonDocument(Replace);
            // changeDoc["$unset"] = new BsonDocument(Remove);
            // return changeDoc;
        }
    }

    public class ItemUpdateOperation
    {
        public ItemUpdateOperationType Operation { get; set; }
        public string Path { get; set; }
        public object Value { get; set; }
    }

    public enum ItemUpdateOperationType
    {
        Replace,
        Remove
    }
}