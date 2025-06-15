using Domain.Models;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IItemApplication
    {
        public Task<BaseResponse<BaseEntityResponse<Item>>> ListItems(BaseFiltersRequest filters);
    }
}
