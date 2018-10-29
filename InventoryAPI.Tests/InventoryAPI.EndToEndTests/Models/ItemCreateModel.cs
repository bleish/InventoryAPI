namespace InventoryAPI.EndToEndTests.Models
{
    public class ItemCreateModel
    {
        public string Name { get; set; }

        public string Model { get; set; }

        public string Category { get; set; }

        public double? Cost { get; set; }

        public string Notes { get; set; }

        public string ReferenceTable { get; set; }
    }
}