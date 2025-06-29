using Domain.Entities;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;

namespace Infraestructure.Persistence.Interfaces
{
    public interface IAuctionRepository
    {
        public Task<BaseEntityResponse<Auction>> ListAuctions(BaseFiltersRequest baseFilter);
        public Task<Auction> GetAuctionById(Guid auctionId);
        public Task CreateAuction(Auction auction);
        public Task<bool> UpdateAuction(Auction auction);
        public Task<bool> DeleteAuction(Auction auction);
    }
}
