using AutoMapper;
using InventoryAPI.Models;
using InventoryAPI.ViewModels;

namespace InventoryAPI.Configuration
{
    public class MappingProfile : Profile
    {
        public MappingProfile() {
            CreateMap<ItemViewModel, Item>();
            CreateMap<Item, ItemReadViewModel>();
            CreateMap<ItemPatchViewModel, Item>().ConvertUsing<ItemTypeConverter>();
        }        
    }

    public class ItemTypeConverter : ITypeConverter<ItemPatchViewModel, Item>
    {
        public Item Convert(ItemPatchViewModel source, Item destination, ResolutionContext context)
        {
            destination.Notes = source.Notes;
            destination.Cost = source.Cost;
            destination.PatchTesting.PatchTest = source.PatchTesting.PatchTest;

            return destination;
        }
    }
}