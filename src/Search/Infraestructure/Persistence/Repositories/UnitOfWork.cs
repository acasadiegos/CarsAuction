using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Domain.Repositories;

namespace Infraestructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IItemRepository<BaseEntityResponse<Item>, ItemFiltersDto> ItemRepository { get; }

        public UnitOfWork()
        {
            ItemRepository = new ItemRepository();
        }
    }
}
