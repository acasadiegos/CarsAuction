using Application.Interfaces;
using Contracts;
using Domain.Models;
using MassTransit;

namespace Infraestructure.Messaging.Consumers
{
    public class AuctionFinishedConsumer : IConsumer<AuctionFinished>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AuctionFinishedConsumer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task Consume(ConsumeContext<AuctionFinished> context)
        {
            var auction = await _unitOfWork.ItemRepository.GetItemByAuctionId(context.Message.AuctionId);

            if(context.Message.ItemSold)
            {
                auction.Winner = context.Message.Winner;
                auction.SoldAmount = (int)context.Message.Amount;
            }

            auction.Status = Status.Finished.ToString();

            await _unitOfWork.ItemRepository.SaveItem(auction);
        }
    }
}
