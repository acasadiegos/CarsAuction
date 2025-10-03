using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Infraestructure.Interfaces;
using Infraestructure.Interfaces.Client;
using Microsoft.Extensions.Configuration;

namespace Infraestructure.Services
{
    public class ItemService : IItemService
    {
        private readonly IConfiguration _configuration;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpClientService _httpClientService;
        public ItemService(IConfiguration configuration, IUnitOfWork unitOfWork, IHttpClientService httpClientService)
        {
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _httpClientService = httpClientService;
        }

        public async Task<IReadOnlyList<Item>> GetItemsForSearchDb()
        {
            string lastUpdated = await _unitOfWork.ItemRepository.GetLastUpdatedDateString();
            string baseUrl = _configuration["Services:Auction:Url"];
            string endpoint = _configuration["Services:Auction:Endpoints:Search"];
            ItemFiltersDto filtersRequest = new ItemFiltersDto
            {
                StartDate = lastUpdated,
                EndDate = DateTime.UtcNow.ToString()
            };

            BaseResponse<BaseEntityResponse<Item>> response = await _httpClientService.Post<ItemFiltersDto, 
                                                                        BaseResponse<BaseEntityResponse<Item>>>
                                                                        (baseUrl, endpoint, filtersRequest);

            return response.Data.Items;
        }
    }
}
