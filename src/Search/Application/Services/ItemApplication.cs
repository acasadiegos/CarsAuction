using Application.Interfaces;
using Domain.Models;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;
using Infraestructure.Persistence.Interfaces;
using Utilities.Static;

namespace Application.Services
{
    public class ItemApplication : IItemApplication
    {
        public IUnitOfWork _unitOfWork;
        public ItemApplication(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<BaseResponse<BaseEntityResponse<Item>>> ListItems(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<Item>>();

            var items = await _unitOfWork.ItemRepository.ListItems(filters);

            if(items is not null && items.Items.Count > 0)
            {
                response.IsSucces = true;
                response.Data = items;
                response.Message = RepplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSucces = false;
                response.Message = RepplyMessage.MESSAGE_QUERY_EMTPY; 
            }

            return response;
        }
    }
}
