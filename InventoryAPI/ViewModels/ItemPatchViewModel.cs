namespace InventoryAPI.ViewModels
{
    public class ItemPatchViewModel
    {
        public double? Cost { get; set; }
        
        public string Notes { get; set; }

        public FetalClassPatchViewModel Fetal { get; set; }
    }

    public class FetalClassPatchViewModel
    {
        public string PandaExpress { get; set; }
    }
}