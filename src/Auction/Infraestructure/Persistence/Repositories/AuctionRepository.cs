using Application.Commons.Bases.Response;
using Application.DTOs;
using Domain.Entities;
using Domain.Repositories;
using Infraestructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Utilities.Helpers;

namespace Infraestructure.Persistence.Repositories;

public class AuctionRepository : IAuctionRepository<BaseEntityResponse<Auction>, AuctionFiltersDto>
{
    private readonly AuctionDbContext _context;

    public AuctionRepository(AuctionDbContext context)
    {
        _context = context;
    }

    public async Task<BaseEntityResponse<Auction>> ListAuctions(AuctionFiltersDto filters)
    {
        var response = new BaseEntityResponse<Auction>();

        bool paginate = filters.NumPage > 0;

        var auctions = _context.Auctions
                        .Include(a => a.Item)
                        .AsNoTracking();

        if (!string.IsNullOrEmpty(filters.Model))
            auctions = auctions.Where(a => a.Item.Model.ToLower().Contains(filters.Model.ToLower()));

        if (filters.Year.HasValue)
            auctions = auctions.Where(a => a.Item.Year == filters.Year.Value);

        if (!string.IsNullOrEmpty(filters.Color))
            auctions = auctions.Where(a => a.Item.Color.ToLower().Contains(filters.Color.ToLower()));

        if (filters.Mileage.MinValue.HasValue)
            auctions = auctions.Where(a => a.Item.Mileage >= filters.Mileage.MinValue);

        if (filters.Mileage.MaxValue.HasValue)
            auctions = auctions.Where(a => a.Item.Mileage <= filters.Mileage.MaxValue);

        if (filters.ReservePrice.MinValue.HasValue)
            auctions = auctions.Where(a => a.ReservePrice >= filters.ReservePrice.MinValue);

        if (filters.ReservePrice.MaxValue.HasValue)
            auctions = auctions.Where(a => a.ReservePrice <= filters.ReservePrice.MaxValue);

        if(filters.StateFilter.HasValue)
            auctions = auctions.Where(a => a.Status == (Status)filters.StateFilter);

        if(!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
        {
            var startDateUtc = DateTime.SpecifyKind(Convert.ToDateTime(filters.StartDate), DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(Convert.ToDateTime(filters.EndDate).AddDays(1), DateTimeKind.Utc);

            auctions = auctions.Where(a => a.UpdatedAt > startDateUtc
                                            && a.CreatedAt <= endDateUtc);
        }

        if(string.IsNullOrEmpty(filters.Sort)) filters.Sort = "Id";

        response.TotalRecords = await auctions.CountAsync();

        response.Items = await auctions.Ordering(filters.Order, filters.Sort, filters.NumPage, filters.Records, paginate).ToListAsync();

        return response;
    }

    public async Task<Auction> GetAuctionById(Guid auctionId)
    {
        return await _context.Auctions
                        .Include(x => x.Item)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.Id == auctionId);
    }

    public async Task CreateAuction(Auction auction)
    {
        await _context.Auctions.AddAsync(auction);
    }   

    public void UpdateAuction(Auction auction)
    {
        _context.Update(auction);
    }

    public void DeleteAuction(Auction auction)
    {
        _context.Remove(auction);
    }
}
