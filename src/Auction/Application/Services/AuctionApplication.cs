using Application.Commons.Base.Response;
using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Application.Messaging.Interfaces;
using Application.Validators.Auction;
using AutoMapper;
using Contracts;
using Domain.Entities;
using System.Net;
using Utilities.Static;

namespace Application.Services
{
    public class AuctionApplication : IAuctionApplication
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly CreateAuctionValidator _createAuctionValidator;
        private readonly UpdateAuctionValidator _updateAuctionValidator;
        private readonly AuctionFiltersValidator _auctionFiltersValidator;
        private readonly IMessagerPublisher _messagerPublisher;

        public AuctionApplication(IUnitOfWork unitOfWork, IMapper mapper, CreateAuctionValidator createAuctionValidator,
            UpdateAuctionValidator updateAuctionValidator, AuctionFiltersValidator auctionFiltersValidator, IMessagerPublisher messagerPublisher)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _createAuctionValidator = createAuctionValidator;
            _updateAuctionValidator = updateAuctionValidator;
            _messagerPublisher = messagerPublisher;
            _auctionFiltersValidator = auctionFiltersValidator;
        }

        public async Task<BaseResponse<BaseEntityResponse<AuctionDto>>> ListAuctions(AuctionFiltersDto filters)
        {
            var response = new BaseResponse<BaseEntityResponse<AuctionDto>>();

            var validationResult = await _auctionFiltersValidator.ValidateAsync(filters);

            if (!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = _mapper.Map<List<ErrorDetail>>(validationResult.Errors);
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var auctions = await _unitOfWork.Auction.ListAuctions(filters);

            if (auctions is not null && auctions.Items.Count > 0)
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

            response.StatusCode = HttpStatusCode.OK;

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
                response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                response.StatusCode = HttpStatusCode.NotFound;
            }

            return response;
        }

        public async Task<BaseCreatedResponse<bool>> CreateAuction(CreateAuctionDto createAuctionDto, string seller)
        {
            var response = new BaseCreatedResponse<bool>();

            var validationResult = await _createAuctionValidator.ValidateAsync(createAuctionDto);

            if(!validationResult.IsValid)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = _mapper.Map<List<ErrorDetail>>(validationResult.Errors);
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            var auction = _mapper.Map<Auction>(createAuctionDto);
            auction.Seller = seller;

            await _unitOfWork.Auction.CreateAuction(auction);

            var auctionDto = _mapper.Map<AuctionDto>(auction);
            var auctionCreated = _mapper.Map<AuctionCreated>(auctionDto);

            await _messagerPublisher.PublishAsync(auctionCreated);

            response.Data = await _unitOfWork.SaveChangesAsync();

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Id = auction.Id;
                response.Message = ReplyMessage.MESSAGE_SAVE;
                response.StatusCode = HttpStatusCode.Created;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
                response.StatusCode = HttpStatusCode.InternalServerError;
                return response;
            }
            
            return response;
        }

        public async Task<BaseResponse<bool>> UpdateAuction(Guid auctionId, UpdateAuctionDto updateAuctionDto, string username)
        {
            var response = new BaseResponse<bool>();

            var auction = await _unitOfWork.Auction.GetAuctionById(auctionId);

            if(auction == null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            if(auction.Seller != username)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FORBID;
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            var validationResult = await _updateAuctionValidator.ValidateAsync(updateAuctionDto);

            if ((!validationResult.IsValid))
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_VALIDATE;
                response.Errors = _mapper.Map<List<ErrorDetail>>(validationResult.Errors);
                response.StatusCode = HttpStatusCode.BadRequest;
                return response;
            }

            _mapper.Map(updateAuctionDto, auction.Item);

            _unitOfWork.Auction.UpdateAuction(auction);

            var auctionUpdated = _mapper.Map<AuctionUpdated>(auction);

            await _messagerPublisher.PublishAsync(auctionUpdated);

            response.Data = await _unitOfWork.SaveChangesAsync();

            if(response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_UPDATE;
                response.StatusCode = HttpStatusCode.OK;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }

        public async Task<BaseResponse<bool>> DeleteAuction(Guid auctionId, string username)
        {
            var response = new BaseResponse<bool>();

            var auction = await _unitOfWork.Auction.GetAuctionById(auctionId);

            if (auction.Seller != username)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FORBID;
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }

            if (auction == null)
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_QUERY_EMPTY;
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }

            _unitOfWork.Auction.DeleteAuction(auction);

            await _messagerPublisher.PublishAsync(new AuctionDeleted { Id = auction.Id.ToString() });

            response.Data = await _unitOfWork.SaveChangesAsync();

            if (response.Data)
            {
                response.IsSuccess = true;
                response.Message = ReplyMessage.MESSAGE_DELETE;
                response.StatusCode = HttpStatusCode.OK;
                return response;
            }
            else
            {
                response.IsSuccess = false;
                response.Message = ReplyMessage.MESSAGE_FAILED;
                response.StatusCode = HttpStatusCode.InternalServerError;
            }

            return response;
        }
    }
}
