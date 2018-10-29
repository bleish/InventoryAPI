using InventoryAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventoryAPI.Repository
{
    public interface IItemsRepository
    {
        Task<IEnumerable<Item>> GetMany();
        Task<Item> GetOne(string id);
        Task<Item> Add(Item item);
        Task<bool> Remove(string id);
        Task<bool> Update(Item item);
    }
}