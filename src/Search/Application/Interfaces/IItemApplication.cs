using Application.Commons.Bases.Response;
using Application.DTOs;
using Domain.Models;

namespace Application.Interfaces
{
    public interface IItemApplication
    {
        public Task<BaseResponse<BaseEntityResponse<Item>>> ListItems(ItemFiltersDto filters);
    }
}
