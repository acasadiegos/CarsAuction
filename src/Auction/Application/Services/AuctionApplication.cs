using Application.Commons.Base;
using Application.DTOs;
using Application.Interfaces;
using Utilities.Static;
using AutoMapper;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;
using Infraestructure.Persistence.Interfaces;
using Domain.Entities;
using Application.Validators.Auction;
using Infraestructure.Messaging.Interfaces;
using Contracts;

namespace Application.Services
{
    public class AuctionApplication : IAuctionApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CreateAuctionValidator _createAuctionValidator;
        private readonly UpdateAuctionValidator _updateAuctionValidator;
        private readonly IMessagerPublisher _messagerPublisher;

        public AuctionApplication(IUnitOfWork unitOfWork, IMapper mapper, CreateAuctionValidator createAuctionValidator,
            UpdateAuctionValidator updateAuctionValidator, IMessagerPublisher messagerPublisher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createAuctionValidator = createAuctionValidator;
            _updateAuctionValidator = updateAuctionValidator;
            _messagerPublisher = messagerPublisher;
        }

        public async Task<BaseResponse<BaseEntityResponse<AuctionDto>>> ListAuctions(BaseFiltersRequest filters)
        {
            var response = new BaseResponse<BaseEntityResponse<AuctionDto>>();

            var auctions = await _unitOfWork.Auction.ListAuctions(filters);

            if(auctions is not null && auctions.Items.Count > 0)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<BaseEntityResponse<AuctionDto>>(auctions);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<AuctionDto>> GetAuctionById(Guid auctionId)
        {
            var response = new BaseResponse<AuctionDto>();

            var auction = await _unitOfWork.Auction.GetAuctionById(auctionId);

            if(auction is not null)
            {
                response.IsSuccess = true;
                response.Data = _mapper.Map<AuctionDto>(auction);
                response.Message = ReplyMessage.MESSAGE_QUERY;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> CreateAuction(CreateAuctionDto createAuctionDto)
        {
            var response = new BaseResponse<bool>();

            var validationResult = await _createAuctionValidator.ValidateAsync(createAuctionDto);

            if(!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;
                return response;
            }

            var auction = _mapper.Map<Auction>(createAuctionDto);

            await _unitOfWork.Auction.CreateAuction(auction);

            var auctionDto = _mapper.Map<AuctionDto>(auction);
            var auctionCreated = _mapper.Map<AuctionCreated>(auctionDto);

            response.Data = await _unitOfWork.SaveChangesAsync();


            if (response.Data)
            {
                await _messagerPublisher.PublishAsync(auctionCreated);
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_SAVE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
                return response;
            }
            
            return response;
        }

        public async Task<BaseResponse<bool>> UpdateAuction(Guid auctionId, UpdateAuctionDto updateAuctionDto)
        {
            var response = new BaseResponse<bool>();

            var auction = await _unitOfWork.Auction.GetAuctionById(auctionId);

            if(auction == null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            var validationResult = await _updateAuctionValidator.ValidateAsync(updateAuctionDto);

            if ((!validationResult.IsValid))
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = validationResult.Errors;
                return response;
            }

            _mapper.Map(updateAuctionDto, auction.Item);

            response.Data = await _unitOfWork.Auction.UpdateAuction(auction);

            if(response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_UPDATE;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAuction(Guid auctionId)
        {
            var response = new BaseResponse<bool>();

            var auction = await _unitOfWork.Auction.GetAuctionById(auctionId);

            if(auction == null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                return response;
            }

            response.Data = await _unitOfWork.Auction.DeleteAuction(auction);

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_DELETE;
                return response;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
            }

            return response;
        }
    }
}
