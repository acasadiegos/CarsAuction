using Application.Commons.Base.Response;
using Application.Commons.Bases.Response;
using Application.DTOs;

namespace Application.Interfaces
{
    public interface IAuctionApplication
    {
        public Task<BaseResponse<BaseEntityResponse<AuctionDto>>> ListAuctions(AuctionFiltersDto filters);
        public Task<BaseResponse<AuctionDto>> GetAuctionById(Guid auctionId);
        public Task<BaseCreatedResponse<bool>> CreateAuction(CreateAuctionDto createAuctionDto, string seller);
        public Task<BaseResponse<bool>> UpdateAuction(Guid auctionId, UpdateAuctionDto updateAuctionDto, string username);
        public Task<BaseResponse<bool>> DeleteAuction(Guid auctionId, string username);
    }
}
