using Infraestructure.Persistence.Interfaces;

namespace Infraestructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        public IItemRepository ItemRepository { get; }

        public UnitOfWork()
        {
            ItemRepository = new ItemRepository();
        }
    }
}
