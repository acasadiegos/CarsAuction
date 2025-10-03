using Application.Commons.Bases.Response;
using Application.DTOs;
using Domain.Entities;
using Domain.Repositories;

namespace Application.Interfaces;

public interface IUnitOfWork: IDisposable
{
    //Declaration of our interfaces at repository level
    IAuctionRepository<BaseEntityResponse<Auction>, AuctionFiltersDto> Auction { get; }
    void SaveChanges();
    Task<bool> SaveChangesAsync();
}
