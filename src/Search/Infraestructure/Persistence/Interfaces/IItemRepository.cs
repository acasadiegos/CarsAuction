using Domain.Models;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;
using MongoDB.Driver;

namespace Infraestructure.Persistence.Interfaces
{
    public interface IItemRepository
    {
        public Task<BaseEntityResponse<Item>> ListItems(BaseFiltersRequest filters);
        public Task<string> GetLastUpdatedDateString();
        public Task SaveItem(Item item);
        public Task<UpdateResult> UpdateItem(string id, Item item);
        public Task<DeleteResult> DeleteItem(string id);
    }
}
