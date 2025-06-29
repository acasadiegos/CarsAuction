using Infraestructure.Persistence.Contexts;
using Infraestructure.Persistence.Interfaces;

namespace Infraestructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AuctionDbContext _context;
    public IAuctionRepository Auction { get; private set; }
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
