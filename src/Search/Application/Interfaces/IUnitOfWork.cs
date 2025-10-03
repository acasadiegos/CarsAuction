using Application.Commons.Bases.Response;
using Application.DTOs;
using Domain.Models;
using Domain.Repositories;

namespace Application.Interfaces;
public interface IUnitOfWork
{
    IItemRepository<BaseEntityResponse<Item>, ItemFiltersDto> ItemRepository { get; }
}
