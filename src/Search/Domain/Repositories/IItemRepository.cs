using Domain.Models;
using MongoDB.Driver;

namespace Domain.Repositories;
public interface IItemRepository<T, X>
{
    public Task<T> ListItems(X filters);
    public Task<string> GetLastUpdatedDateString();
    public Task SaveItem(Item item);
    public Task<UpdateResult> UpdateItem(string id, Item item);
    public Task<DeleteResult> DeleteItem(string id);
}

