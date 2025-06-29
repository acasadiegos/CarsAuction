using Domain.Entities;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;
using Infraestructure.Helpers;
using Infraestructure.Persistence.Contexts;
using Infraestructure.Persistence.Interfaces;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Repositories;

public class AuctionRepository : IAuctionRepository
{
    private readonly AuctionDbContext _context;

    public AuctionRepository(AuctionDbContext context)
    {
        _context = context;
    }
    public async Task<BaseEntityResponse<Auction>> ListAuctions(BaseFiltersRequest baseFilter)
    {
        var response = new BaseEntityResponse<Auction>();

        bool paginate = baseFilter.NumPage > 0;

        var auctions = _context.Auctions
                        .Include(a => a.Item)
                        .AsNoTracking();

        foreach (var filter in baseFilter.Filters)
        {
            int numFilterValue = 0;

            if(filter.NumFilter != 0 && !string.IsNullOrEmpty(filter.TextFilter))
            {
                switch(filter.NumFilter)
                {
                    case 1:
                        auctions = auctions.Where(a => a.Item.Model.ToLower().Contains(filter.TextFilter.ToLower()));
                        break;
                    case 2:
                        int.TryParse(filter.TextFilter, out numFilterValue);
                        auctions = auctions.Where(a => a.Item.Year.Equals(numFilterValue));
                        break;
                    case 3:
                        auctions = auctions.Where(a => a.Item.Color.ToLower().Contains(filter.TextFilter.ToLower()));
                        break;
                }
            }
            else if(filter.NumFilter != 0 && filter.MinNumberValue >= 0 && filter.MaxNumberValue >= 0 )
            {
                switch(filter.NumFilter)
                {
                    case 4:
                        auctions = auctions.Where( a => a.Item.Mileage >= filter.MinNumberValue && 
                                                a.Item.Mileage <= filter.MaxNumberValue);
                        break;
                    case 5:
                        auctions = auctions.Where( a => a.ReservePrice >= filter.MinNumberValue && 
                                                a.ReservePrice <= filter.MaxNumberValue);
                        break;
                }
            }
        }

        if(baseFilter.StateFilter >= 1)
        {
            auctions = auctions.Where(a => a.Status == (Status)baseFilter.StateFilter);
        }

        if(!string.IsNullOrEmpty(baseFilter.StartDate) && !string.IsNullOrEmpty(baseFilter.EndDate))
        {
            var startDateUtc = DateTime.SpecifyKind(Convert.ToDateTime(baseFilter.StartDate), DateTimeKind.Utc);
            var endDateUtc = DateTime.SpecifyKind(Convert.ToDateTime(baseFilter.EndDate).AddDays(1), DateTimeKind.Utc);

            auctions = auctions.Where(a => a.UpdatedAt > startDateUtc
                                            && a.CreatedAt <= endDateUtc);
        }

        if(string.IsNullOrEmpty(baseFilter.Sort)) baseFilter.Sort = "Id";

        response.TotalRecords = await auctions.CountAsync();

        response.Items = await Ordering(baseFilter, auctions, paginate).ToListAsync();

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

    public async Task<bool> UpdateAuction(Auction auction)
    {
        _context.Update(auction);
        var recordsAffected = await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        return recordsAffected > 0;
    }

    public async Task<bool> DeleteAuction(Auction auction)
    {
        _context.Remove(auction);
        var recordsAffected = await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        return recordsAffected > 0;
    }

    public IQueryable<TDTO> Ordering<TDTO>(BasePaginationRequest request, IQueryable<TDTO> queryable, bool pagination = false) where TDTO : class
    {
        IQueryable<TDTO> queryDto = request.Order == "desc" ? queryable.OrderBy($"{request.Sort} descending") : queryable.OrderBy($"{request.Sort} ascending");

        if(pagination)
        {
            queryDto = queryDto.Paginate(request);
        }

        return queryDto;
    }
}
