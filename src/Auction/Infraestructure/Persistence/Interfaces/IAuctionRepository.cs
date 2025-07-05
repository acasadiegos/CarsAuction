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
        public void UpdateAuction(Auction auction);
        public void DeleteAuction(Auction auction);
    }
}
