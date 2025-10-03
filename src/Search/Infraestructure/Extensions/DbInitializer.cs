using Domain.Models;
using Infraestructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Entities;

namespace Infraestructure.Extensions
{
    public class DbInitializer
    {
        public static async Task InitDb(IConfiguration configuration, IServiceScope scope)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
                .FromConnectionString(configuration.GetConnectionString("MongoDbConnection")));

            await DB.Index<Item>()
                .Key(x => x.Make, KeyType.Text)
                .Key(x => x.Model, KeyType.Text)
                .Key(x => x.Color, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Item>();

            var itemService = scope.ServiceProvider.GetRequiredService<IItemService>();

            var items = await itemService.GetItemsForSearchDb();

            Console.WriteLine(items.Count + " returned from the auction service");

            if(items.Count > 0)
            {
                await DB.SaveAsync(items);
            }

        }
    }
}
