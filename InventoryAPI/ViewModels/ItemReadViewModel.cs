using MongoDB.Bson;

namespace InventoryAPI.ViewModels
{
    public class ItemReadViewModel
    {
        /// <summary>
        /// The item ID (ObjectId format).
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The item name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The item model.
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// The item category (e.g. Electronics, Furniture, etc.).
        /// </summary>        
        public string Category { get; set; }

        /// <summary>
        /// The item cost (value at purchase).
        /// </summary>
        public double? Cost { get; set; }

        /// <summary>
        /// Additional details abou the item.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// The name of the dedicated db table/collection for this item (e.g. Television, Vehicle, etc.).
        /// </summary>
        public string ReferenceTable { get; set; }

        /// <summary>
        /// This is a temporary property used to test the patch operation
        /// </summary>
        public PatchTestingReadViewModel PatchTesting { get; set; }
    }

    public class PatchTestingReadViewModel
    {
        public string PatchTest { get; set; }
    }
}