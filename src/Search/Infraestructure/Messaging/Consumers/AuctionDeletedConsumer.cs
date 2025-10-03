using Application.Interfaces;
using Contracts;
using MassTransit;

namespace Infraestructure.Messaging.Consumers
{
    public class AuctionDeletedConsumer : IConsumer<AuctionDeleted>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuctionDeletedConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Consume(ConsumeContext<AuctionDeleted> context)
        {
            Console.WriteLine("--> Consuming AuctionDeleted: " + context.Message.Id);

            var result = await _unitOfWork.ItemRepository.DeleteItem(context.Message.Id);

            if (!result.IsAcknowledged)
                throw new MessageException(typeof(AuctionDeleted), "Problem deleting auction");
        }
    }
}
