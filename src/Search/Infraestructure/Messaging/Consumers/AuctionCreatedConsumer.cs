using Application.Interfaces;
using AutoMapper;
using Contracts;
using Domain.Models;
using MassTransit;

namespace Infraestructure.Messaging.Consumers
{
    public class AuctionCreatedConsumer : IConsumer<AuctionCreated>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public AuctionCreatedConsumer(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            Console.WriteLine("--> Consuming auction created: " + context.Message.Id);

            var item = _mapper.Map<Item>(context.Message);

            await _unitOfWork.ItemRepository.SaveItem(item);
        }
    }
}
