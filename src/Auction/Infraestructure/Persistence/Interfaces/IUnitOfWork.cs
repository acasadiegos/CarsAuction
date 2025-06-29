namespace Infraestructure.Persistence.Interfaces;

public interface IUnitOfWork: IDisposable
{
    //Declaration of our interfaces at repository level
    IAuctionRepository Auction { get; }
    void SaveChanges();
    Task<bool> SaveChangesAsync();
}
