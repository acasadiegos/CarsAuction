using Application.Interfaces;
using Contracts;
using MassTransit;

namespace Infraestructure.Messaging.Consumers
{
    public class BidPlacedConsumer : IConsumer<BidPlaced>
    {
        private readonly IUnitOfWork _unitOfWork;

        public BidPlacedConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Consume(ConsumeContext<BidPlaced> context)
        {
            Console.WriteLine("--> Consuming bid placed");

            var auction = await _unitOfWork.ItemRepository.GetItemByAuctionId(context.Message.AuctionId);

            if (context.Message.BidStatus.Contains("Acepted") && context.Message.Amount > auction.CurrentHighBid)
            {
                auction.CurrentHighBid = context.Message.Amount;
                await _unitOfWork.ItemRepository.SaveItem(auction);
            }
        }
    }
}
