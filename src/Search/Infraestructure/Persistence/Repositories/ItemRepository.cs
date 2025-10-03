using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.DTOs;
using Domain.Models;
using Domain.Repositories;
using Infraestructure.Helpers;
using MongoDB.Driver;
using MongoDB.Entities;

namespace Infraestructure.Persistence.Repositories
{
    public class ItemRepository: IItemRepository<BaseEntityResponse<Item>, ItemFiltersDto>
    {
        public async Task<BaseEntityResponse<Item>> ListItems(ItemFiltersDto filters)
        {
            var response = new BaseEntityResponse<Item>();
            string defaultSortField = "Id";
            bool paginate = filters.NumPage > 0;
            bool hasTextScore = false;

            var items = DB.PagedSearch<Item,Item>();

            if (!string.IsNullOrEmpty(filters.Model))
                items = items.Match(i => i.Model.ToLower().Contains(filters.Model.ToLower()));

            if (filters.Year.HasValue)
                items = items.Match(i => i.Year.Equals(filters.Year));

            if (!string.IsNullOrEmpty(filters.Color))
                items = items.Match(i => i.Color.ToLower().Contains(filters.Color.ToLower()));

            if(!string.IsNullOrEmpty(filters.Make))
                items = items.Match(i => i.Make.ToLower().Contains(filters.Make.ToLower()));

            if (!string.IsNullOrEmpty(filters.Winner))
                items = items.Match(i => i.Winner.ToLower().Contains(filters.Winner.ToLower()));

            if (!string.IsNullOrEmpty(filters.Seller))
                items = items.Match(i => i.Seller.ToLower().Contains(filters.Seller.ToLower()));

            if (filters.Mileage.MinValue.HasValue)
                items = items.Match(i => i.Mileage >= filters.Mileage.MinValue);

            if (filters.Mileage.MaxValue.HasValue)
                items = items.Match(i => i.Mileage <= filters.Mileage.MaxValue);

            if (filters.ReservePrice.MinValue.HasValue)
                items = items.Match(i => i.ReservePrice >= filters.ReservePrice.MinValue);

            if (filters.ReservePrice.MaxValue.HasValue)
                items = items.Match(i => i.ReservePrice <= filters.ReservePrice.MaxValue);

            if (filters.OnlyGetEndingSoonAuctions)
                items = items.Match(i => i.AuctionEnd < DateTime.UtcNow.AddHours(6) && i.AuctionEnd > DateTime.UtcNow);

            if (!string.IsNullOrEmpty(filters.FullSearchText))
            {
                items = items.Match(Search.Full, filters.FullSearchText);
                hasTextScore = true;
            }

            if (filters.StateFilter.HasValue)
                items.Match(i => i.Status == ((Status)filters.StateFilter).ToString());

            if(!string.IsNullOrEmpty(filters.StartDate) && !string.IsNullOrEmpty(filters.EndDate))
            {
                var startDateUtc = DateTime.SpecifyKind(Convert.ToDateTime(filters.StartDate), DateTimeKind.Utc);
                var endDateUtc = DateTime.SpecifyKind(Convert.ToDateTime(filters.EndDate).AddDays(1), DateTimeKind.Utc);

                items.Match(i => i.CreatedAt >= startDateUtc && i.CreatedAt <= endDateUtc);
            }

            if (hasTextScore)
            {
                items.SortByTextScore();
            }

            if (string.IsNullOrEmpty(filters.Sort)) filters.Sort = defaultSortField;

            if(paginate)
            {
                items.PageNumber(filters.NumPage);
                items.PageSize(filters.Records);
            }

            items = items.ApplyOrdering(filters, defaultSortField);

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

        public async Task SaveItem(Item item)
        {
            await item.SaveAsync();
        }

        public async Task<UpdateResult> UpdateItem(string id, Item item)
        {
            return await DB.Update<Item>()
                            .Match(a => a.ID == id)
                            .ModifyOnly(x => new
                            {
                                x.Color,
                                x.Make,
                                x.Model,
                                x.Year,
                                x.Mileage,
                            }, item)
                            .ExecuteAsync();

        }

        public async Task<DeleteResult> DeleteItem(string id)
        {
            return await DB.DeleteAsync<Item>(id);
        }
    }
}
