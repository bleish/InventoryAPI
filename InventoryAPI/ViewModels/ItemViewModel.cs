using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.ViewModels
{
    public class ItemViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Model { get; set; }
        
        [Required]
        public string Category { get; set; }

        public double? Cost { get; set; }

        public string Notes { get; set; }

        public string ReferenceTable { get; set; }
    }
}