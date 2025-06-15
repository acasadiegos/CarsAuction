using Domain.Models;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;

namespace Infraestructure.Persistence.Interfaces
{
    public interface IItemRepository
    {
        public Task<BaseEntityResponse<Item>> ListItems(BaseFiltersRequest filters);

        public Task<string> GetLastUpdatedDateString();
    }
}
