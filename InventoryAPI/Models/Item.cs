using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.Models
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("name")]
        [Required]
        public string Name { get; set; }

        [BsonElement("model")]
        [Required]
        public string Model { get; set; }

        [BsonElement("category")]
        [Required]
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