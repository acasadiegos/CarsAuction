using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Application.Validators.Auction;
using Domain.Models;
using System.Net;
using Utilities.Static;

namespace Application.Services
{
    public class ItemApplication : IItemApplication
    {
        public IUnitOfWork _unitOfWork;
        private readonly ItemFiltersValidator _itemFiltersValidator;

        public ItemApplication(IUnitOfWork unitOfWork, ItemFiltersValidator itemFiltersValidator)
        {
            _unitOfWork = unitOfWork;
            _itemFiltersValidator = itemFiltersValidator;
        }
        public async Task<BaseResponse<BaseEntityResponse<Item>>> ListItems(ItemFiltersDto filters)
        {
            var response = new BaseResponse<BaseEntityResponse<Item>>();

            var validationResult = await _itemFiltersValidator.ValidateAsync(filters);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = RepplyMessage.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors
                    .Select(e => new ErrorDetail
                    {
                        PropertyName = e.PropertyName,
                        ErrorMessage = e.ErrorMessage
                    }).ToList();
                response.StatusCode = HttpStatusCode.BadRequest;

                return response;
            }

            var items = await _unitOfWork.ItemRepository.ListItems(filters);

            if(items is not null && items.Items.Count > 0)
            {
                response.IsSuccess = true;
                response.Data = items;
                response.Message = RepplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = RepplyMessage.MESSAGE_QUERY_EMTPY; 
            }

            response.StatusCode = HttpStatusCode.OK;

            return response;
        }
    }
}
