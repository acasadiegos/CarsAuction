using Application.Commons.Base;
using Application.DTOs;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;

namespace Application.Interfaces
{
    public interface IAuctionApplication
    {
        public Task<BaseResponse<BaseEntityResponse<AuctionDto>>> ListAuctions(BaseFiltersRequest filters);
        public Task<BaseResponse<AuctionDto>> GetAuctionById(Guid auctionId);
        public Task<BaseResponse<bool>> CreateAuction(CreateAuctionDto createAuctionDto);
        public Task<BaseResponse<bool>> UpdateAuction(Guid auctionId, UpdateAuctionDto updateAuctionDto);
        public Task<BaseResponse<bool>> DeleteAuction(Guid auctionId);
    }
}
