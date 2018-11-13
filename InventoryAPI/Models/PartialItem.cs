using MongoDB.Bson.Serialization.Attributes;

namespace InventoryAPI.Models
{
    public class PartialItem
    {
        [BsonElement("id")]
        [BsonIgnoreIfNull]
        public string Id { get; set; }

        [BsonElement("name")]
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [BsonElement("model")]
        [BsonIgnoreIfNull]
        public string Model { get; set; }

        [BsonElement("category")]
        [BsonIgnoreIfNull]
        public string Category { get; set; }

        [BsonElement("cost")]
        [BsonIgnoreIfNull]
        public double? Cost { get; set; }

        [BsonElement("notes")]
        [BsonIgnoreIfNull]
        public string Notes { get; set; }

        [BsonElement("referenceTable")]
        [BsonIgnoreIfNull]
        public string ReferenceTable { get; set; }
    }
}