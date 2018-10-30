using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.ViewModels
{
    public class ItemViewModel
    {
        /// <summary>
        /// The item name.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// The item model.
        /// </summary>
        [Required]
        public string Model { get; set; }
        
        /// <summary>
        /// The item category (e.g. Electronics, Furniture, etc.).
        /// </summary>
        [Required]
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
    }
}