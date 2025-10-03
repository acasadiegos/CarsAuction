using Application.Interfaces;
using AutoMapper;
using Contracts;
using Domain.Models;
using MassTransit;

namespace Infraestructure.Messaging.Consumers
{
    public class AuctionUpdatedConsumer : IConsumer<AuctionUpdated>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AuctionUpdatedConsumer(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task Consume(ConsumeContext<AuctionUpdated> context)
        {
            Console.WriteLine("--> Consuming auction updated: " + context.Message.Id);

            var item = _mapper.Map<Item>(context.Message);

            var result = await _unitOfWork.ItemRepository.UpdateItem(context.Message.Id, item);

            if (!result.IsAcknowledged)
                throw new MessageException(typeof(AuctionUpdated), "Problem updating mongodb");
        }
    }
}
