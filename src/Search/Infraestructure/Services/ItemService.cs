using Domain.Models;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;
using Infraestructure.Interfaces;
using Infraestructure.Interfaces.Client;
using Infraestructure.Persistence.Interfaces;
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
            BaseFiltersRequest baseFiltersRequest = new BaseFiltersRequest
            {
                StartDate = lastUpdated,
                EndDate = DateTime.UtcNow.ToString()
            };

            BaseResponse<BaseEntityResponse<Item>> response = await _httpClientService.Post<BaseFiltersRequest, 
                                                                        BaseResponse<BaseEntityResponse<Item>>>
                                                                        (baseUrl, string.Empty, baseFiltersRequest);

            return response.Data.Items;
        }
    }
}
