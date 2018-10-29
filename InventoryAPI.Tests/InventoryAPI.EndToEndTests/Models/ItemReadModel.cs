namespace InventoryAPI.EndToEndTests.Models
{
    internal class ItemReadModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Category { get; set; }

        public double? Cost { get; set; }

        public string Notes { get; set; }

        public string ReferenceTable { get; set; }
    }
}