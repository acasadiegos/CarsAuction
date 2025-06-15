namespace Infraestructure.Persistence.Interfaces
{
    public interface IUnitOfWork
    {
        IItemRepository ItemRepository { get; }
    }
}
