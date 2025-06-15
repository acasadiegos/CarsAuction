using Domain.Models;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;
using Infraestructure.Helpers;
using Infraestructure.Persistence.Interfaces;
using MongoDB.Entities;

namespace Infraestructure.Persistence.Repositories
{
    public class ItemRepository: IItemRepository
    {
        public async Task<BaseEntityResponse<Item>> ListItems(BaseFiltersRequest baseFilters)
        {
            var response = new BaseEntityResponse<Item>();
            string defaultSortField = "Id";
            bool paginate = baseFilters.NumPage > 0;
            bool hasTextScore = false;

            var items = DB.PagedSearch<Item,Item>();

            foreach(var filter in baseFilters.Filters)
            {
                int numFilterValue = 0;

                if(filter.NumFilter != 0 && !string.IsNullOrEmpty(filter.TextFilter)) //Text filters
                {
                    int.TryParse(filter.TextFilter, out numFilterValue);

                    items = filter.NumFilter switch
                    {
                        1 => items.Match(i => i.Model.ToLower().Contains(filter.TextFilter.ToLower())),
                        2 => items.Match(i => i.Year.Equals(numFilterValue)),
                        3 => items.Match(i => i.Color.ToLower().Contains(filter.TextFilter.ToLower())),
                        4 => items.Match(i => i.Make.ToLower().Contains(filter.TextFilter.ToLower())),
                        5 => items.Match(i => i.Winner.ToLower().Contains(filter.TextFilter.ToLower())),
                        6 => items.Match(i => i.Seller.ToLower().Contains(filter.TextFilter.ToLower())),
                        _ => items
                    };
                }
                else if(filter.NumFilter != 0 && filter.MinNumberValue >= 0 && filter.MaxNumberValue >= 0) //Filter by numeric range
                {
                    items = filter.NumFilter switch
                    {
                        7 => items.Match(i => i.Mileage >= filter.MinNumberValue && i.Mileage <= filter.MaxNumberValue),
                        8 => items.Match(i => i.ReservePrice >= filter.MinNumberValue && i.ReservePrice <= filter.MaxNumberValue),
                        _ => items
                    };

                }
                else if (baseFilters.Filters.Count == 1 && filter.NumFilter == 0 //Full search
                    && !string.IsNullOrEmpty(filter.TextFilter))
                {
                    items.Match(Search.Full, filter.TextFilter);
                    hasTextScore = true;
                }
                else //Customized filters
                {
                    items = filter.NumFilter switch
                    {
                        9 => items.Match(i => i.AuctionEnd < DateTime.UtcNow.AddHours(6) && i.AuctionEnd > DateTime.UtcNow), // Ending soon auctions
                        _ => items
                    };
                }
            }

            if(baseFilters.StateFilter >= 1)
            {
                items.Match(i => i.Status == ((Status)baseFilters.StateFilter).ToString());
            }

            if(!string.IsNullOrEmpty(baseFilters.StartDate) && !string.IsNullOrEmpty(baseFilters.EndDate))
            {
                var startDateUtc = DateTime.SpecifyKind(Convert.ToDateTime(baseFilters.StartDate), DateTimeKind.Utc);
                var endDateUtc = DateTime.SpecifyKind(Convert.ToDateTime(baseFilters.EndDate).AddDays(1), DateTimeKind.Utc);

                items.Match(i => i.CreatedAt >= startDateUtc && i.CreatedAt <= endDateUtc);
            }

            if (hasTextScore)
            {
                items.SortByTextScore();
            }

            if (string.IsNullOrEmpty(baseFilters.Sort)) baseFilters.Sort = defaultSortField;

            if(paginate)
            {
                items.PageNumber(baseFilters.NumPage);
                items.PageSize(baseFilters.Records);
            }

            items = items.ApplyOrdering(baseFilters, defaultSortField);

            var result = await items.ExecuteAsync();

            response.Items = result.Results;
            response.TotalRecords = result.TotalCount;

            return response;

        }

        public async Task<string> GetLastUpdatedDateString()
        {
            return await DB.Find<Item, string>()
                .Sort(x => x.Descending(x => x.UpdatedAt))
                .Project(x => x.UpdatedAt.ToString())
                .ExecuteFirstAsync();
        }
    }
}
