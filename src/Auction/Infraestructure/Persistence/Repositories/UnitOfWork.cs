using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Infraestructure.Persistence.Contexts;

namespace Infraestructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuctionDbContext _context;
    public IAuctionRepository<BaseEntityResponse<Auction>, AuctionFiltersDto> Auction { get; private set; }
    public UnitOfWork(AuctionDbContext context)
    {
        _context = context;
        Auction = new AuctionRepository(context);
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public void SaveChanges()
    {
        _context.SaveChanges();
    }

    public async Task<bool> SaveChangesAsync()
    {
        var recordsAffected = await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();
        return recordsAffected > 0;
    }
}
