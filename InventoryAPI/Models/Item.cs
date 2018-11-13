using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.Models
{
    public class Item
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get; set; }

        [BsonElement("name")]
        [Required]
        public virtual string Name { get; set; }

        [BsonElement("model")]
        [Required]
        public virtual string Model { get; set; }

        [BsonElement("category")]
        [Required]
        public virtual string Category { get; set; }

        [BsonElement("cost")]
        [BsonIgnoreIfNull]
        public virtual double? Cost { get; set; }

        [BsonElement("notes")]
        [BsonIgnoreIfNull]
        public virtual string Notes { get; set; }

        [BsonElement("referenceTable")]
        [BsonIgnoreIfNull]
        public virtual string ReferenceTable { get; set; }

        [BsonElement("fetal")]
        [BsonIgnoreIfNull]
        public virtual FetalClass Fetal { get; set; }
    }

    public class FetalClass
    {
        public virtual string PandaExpress { get; set; }
    }
}