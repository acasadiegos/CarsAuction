using Domain.Entities;

namespace Domain.Repositories
{
    public interface IAuctionRepository<T,X>
    {
        public Task<T> ListAuctions(X filters);
        public Task<Auction> GetAuctionById(Guid auctionId);
        public Task CreateAuction(Auction auction);
        public void UpdateAuction(Auction auction);
        public void DeleteAuction(Auction auction);
    }
}
