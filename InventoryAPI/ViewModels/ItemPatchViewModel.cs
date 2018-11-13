namespace InventoryAPI.ViewModels
{
    public class ItemPatchViewModel
    {
        public double? Cost { get; set; }
        
        public string Notes { get; set; }

        public PatchTestingPatchViewModel PatchTesting { get; set; }
    }

    public class PatchTestingPatchViewModel
    {
        public string PatchTest { get; set; }
    }
}