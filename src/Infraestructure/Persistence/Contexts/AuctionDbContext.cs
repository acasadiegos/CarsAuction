using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Persistence.Contexts;

public class AuctionDbContext : DbContext
{
    public AuctionDbContext(DbContextOptions options) : base(options)
    {

    }

    public DbSet<Auction> Auctions { get; set; }
}
