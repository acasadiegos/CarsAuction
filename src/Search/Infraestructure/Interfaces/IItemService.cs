using Domain.Models;

namespace Infraestructure.Interfaces
{
    public interface IItemService
    {
        public Task<IReadOnlyList<Item>> GetItemsForSearchDb();
    }
}
