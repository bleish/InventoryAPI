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
        }        
    }
}