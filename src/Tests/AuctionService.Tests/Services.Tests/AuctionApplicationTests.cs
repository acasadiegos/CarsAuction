using Application.Commons.Base.Response;
using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Application.Messaging.Interfaces;
using Application.Services;
using AutoMapper;
using Contracts;
using Domain.Entities;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Net;
using Utilities.Static;

namespace AuctionService.Tests.Services.Test
{
    public class AuctionApplicationTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IValidator<AuctionFiltersDto>> _auctionFiltersValidatorMock = new();
        private readonly Mock<IValidator<CreateAuctionDto>> _createAuctionValidatorMock = new();
        private readonly Mock<IValidator<UpdateAuctionDto>> _updateAuctionValidatorMock = new();
        private readonly Mock<IMessagerPublisher> _messagePublisherMock = new();
        private const string _seller = "bob";
        private const string _anotherSeller = "alice";
        private const string _propertyNameExample = "Field";
        private const string _errorMessageExample = "Error messagge";

        private AuctionApplication CreateService()
        {
            return new AuctionApplication(
                  _unitOfWorkMock.Object,
                  _mapperMock.Object,
                  _createAuctionValidatorMock.Object, 
                  _updateAuctionValidatorMock.Object, 
                  _auctionFiltersValidatorMock.Object,
                  _messagePublisherMock.Object
            );
        }

        [Fact]
        public async Task ListAuctions_WhenDataIsNotValid_BadRequest()
        {
            //Arrange
            var filters = new AuctionFiltersDto();

            var error = new ErrorDetail();
            error.PropertyName = _propertyNameExample;
            error.ErrorMessage = _errorMessageExample;

            var validationResult = new ValidationResult(
                    new List<ValidationFailure>()
                    {
                        new ValidationFailure(error.PropertyName, error.ErrorMessage)
                    }
                );

            var errors = new List<ErrorDetail>() { error };

            _auctionFiltersValidatorMock.Setup(v => v.ValidateAsync(filters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mapperMock.Setup(m => m.Map<List<ErrorDetail>>(validationResult.Errors))
                .Returns(errors);

            AuctionApplication _auctionApplicationService = CreateService();

            //Act
            var response = await _auctionApplicationService.ListAuctions(filters);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, response.Message);
            Assert.Equal(errors, response.Errors);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ListAuctions_WhenFiltersReturnAuctions_Ok()
        {
            //Arrange
            var auction = new Auction();
            var filters = new AuctionFiltersDto();
            var auctions = new BaseEntityResponse<Auction>();
            auctions.Items = new List<Auction>() { auction };
            var auctionsDto = new BaseEntityResponse<AuctionDto>();

            _auctionFiltersValidatorMock.Setup(v => v.ValidateAsync(filters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _unitOfWorkMock.Setup(u => u.Auction.ListAuctions(filters))
                .ReturnsAsync(auctions);

            _mapperMock.Setup(m => m.Map<BaseEntityResponse<AuctionDto>>(auctions))
                .Returns(auctionsDto);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.ListAuctions(filters);

            //Assert
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Data);
            Assert.Equal(ReplyMessage.MESSAGE_QUERY, response.Message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task ListAuctions_WhenFiltersDontReturnAuctions_Ok()
        {
            //Arrange
            var filters = new AuctionFiltersDto();
            var auctions = new BaseEntityResponse<Auction>();
            auctions.Items = new List<Auction>();

            _auctionFiltersValidatorMock.Setup(v => v.ValidateAsync(filters, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _unitOfWorkMock.Setup(u => u.Auction.ListAuctions(filters))
                .ReturnsAsync(auctions);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.ListAuctions(filters);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Null(response.Data);
            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, response.Message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAuctionById_WhenAuctionExists_Ok()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction();
            var auctionDto = new AuctionDto();

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            _mapperMock.Setup(m => m.Map<AuctionDto>(auction))
                .Returns(auctionDto);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.GetAuctionById(auctionId);

            //Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.Equal(ReplyMessage.MESSAGE_QUERY, response.Message); 

        }

        [Fact]
        public async Task GetAuctionById_WhenAuctionDoesNotExists_NotFound()
        {
            //Arrange
            var auctionId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync((Auction)null);

            var auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.GetAuctionById(auctionId);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, response.Message);
            Assert.Null(response.Data);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAuctionById_WhenAuctionExists_CallMapperOnce()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction();
            var auctionDto = new AuctionDto();

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            _mapperMock.Setup(m => m.Map<AuctionDto>(auction))
                .Returns(auctionDto);

            var auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.GetAuctionById(auctionId);

            //Arrange

            _mapperMock.Verify(
                m => m.Map<AuctionDto>(auction),
                Times.Once
            );

            Assert.True(response.IsSuccess);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WhenDataIsNotValid_BadRequest()
        {
            //Arrange
            var createAuctionDto = new CreateAuctionDto();
            var error = new ErrorDetail()
            {
                PropertyName = _propertyNameExample,
                ErrorMessage = _errorMessageExample
            };
            var validationFailures = new List<ValidationFailure>() { 
                
                new ValidationFailure()
                {
                    PropertyName = error.PropertyName,
                    ErrorMessage = error.ErrorMessage
                }
            };
            var validationResult = new ValidationResult(validationFailures);
            var errors = new List<ErrorDetail>() { error };


            _createAuctionValidatorMock.Setup(v => v.ValidateAsync(createAuctionDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mapperMock.Setup(m => m.Map<List<ErrorDetail>>(validationResult.Errors))
                .Returns(errors);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.CreateAuction(createAuctionDto, _seller);


            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, response.Message);
            Assert.Equal(errors, response.Errors);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WhenAuctionCreated_Created()
        {
            //Arrange
            var createAuctionDto = new CreateAuctionDto();
            var auction = new Auction();
            var auctionDto = new AuctionDto();
            var auctionCreated = new AuctionCreated();

            _createAuctionValidatorMock.Setup(v => v.ValidateAsync(createAuctionDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mapperMock.Setup(m => m.Map<Auction>(createAuctionDto))
                .Returns(auction);

            _mapperMock.Setup(m => m.Map<AuctionDto>(auction))
                .Returns(auctionDto);

            _mapperMock.Setup(m => m.Map<AuctionCreated>(auctionDto))
                .Returns(auctionCreated);

            _unitOfWorkMock.Setup(u => u.Auction.CreateAuction(auction));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(true);

            _messagePublisherMock.Setup(mp => mp.PublishAsync(auctionCreated, It.IsAny<CancellationToken>()));

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.CreateAuction(createAuctionDto, _seller);

            //Assert
            Assert.True(response.IsSuccess);
            Assert.NotNull(response.Id);
            Assert.Equal(ReplyMessage.MESSAGE_SAVE, response.Message);
            Assert.True(response.Data);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreateAuction_WhenAuctionNotCreated_InternalServerError()
        {
            //Arrange
            var createAuctionDto = new CreateAuctionDto();
            var auction = new Auction();
            var auctionDto = new AuctionDto();
            var auctionCreated = new AuctionCreated();


            _createAuctionValidatorMock.Setup(v => v.ValidateAsync(createAuctionDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mapperMock.Setup(m => m.Map<Auction>(createAuctionDto))
                 .Returns(auction);

            _mapperMock.Setup(m => m.Map<AuctionDto>(auction))
                .Returns(auctionDto);

            _mapperMock.Setup(m => m.Map<AuctionCreated>(auctionDto))
                .Returns(auctionCreated);

            _unitOfWorkMock.Setup(u => u.Auction.CreateAuction(auction));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(false);

            _messagePublisherMock.Setup(mp => mp.PublishAsync(auctionCreated, It.IsAny<CancellationToken>()));

            AuctionApplication auctionApplicationService = CreateService();


            //Act
            var response = await auctionApplicationService.CreateAuction(createAuctionDto, _seller);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.False(response.Data);
            Assert.Equal(ReplyMessage.MESSAGE_FAILED, response.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuction_WhenAuctionDoesNotExists_NotFound()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var updateAuctionDto = new UpdateAuctionDto();

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync((Auction)null);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.UpdateAuction(auctionId, updateAuctionDto, _seller);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, response.Message);
            Assert.False(response.Data);
            Assert.Null(response.Errors);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateAuction_WhenUserIsNotAuctionOwner_Forbidden()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var updateAuctionDto = new UpdateAuctionDto();
            var auction = new Auction();
            auction.Seller = _anotherSeller;

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.UpdateAuction(auctionId, updateAuctionDto, _seller);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_FORBID, response.Message);
            Assert.False(response.Data);
            Assert.Null(response.Errors);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        }

        [Fact]
        public async Task UpdateAuction_WhenDataIsNotValid_BadRequest()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction();
            auction.Seller = _seller;
            var updateAuctionDto = new UpdateAuctionDto();
            var error = new ErrorDetail();
            error.PropertyName = _propertyNameExample;
            error.ErrorMessage = _errorMessageExample;
            var errors = new List<ErrorDetail>() { error };
            var validationFailures = new List<ValidationFailure>()
            {
                new ValidationFailure()
                {
                    PropertyName = error.PropertyName,
                    ErrorMessage = error.ErrorMessage
                }
            };
            var validationResult = new ValidationResult(validationFailures);

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            _updateAuctionValidatorMock.Setup(v => v.ValidateAsync(updateAuctionDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _mapperMock.Setup(m => m.Map<List<ErrorDetail>>(validationFailures))
                .Returns(errors);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.UpdateAuction(auctionId, updateAuctionDto, _seller);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_VALIDATE, response.Message);
            Assert.Equal(errors, response.Errors);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.False(response.Data);
        }

        [Fact]
        public async Task UpdateAuction_WhenAuctionUpdated_Ok()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var updateAuctionDto = new UpdateAuctionDto();
            var auction = new Auction();
            var auctionUpdated = new AuctionUpdated();
            auction.Seller = _seller;

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            _unitOfWorkMock.Setup(u => u.Auction.UpdateAuction(auction));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(true);

            _updateAuctionValidatorMock.Setup(v => v.ValidateAsync(updateAuctionDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mapperMock.Setup(m => m.Map(updateAuctionDto, auction.Item));

            _mapperMock.Setup(m => m.Map<AuctionUpdated>(auction))
                .Returns(auctionUpdated);

            _messagePublisherMock.Setup(mp => mp.PublishAsync(auctionUpdated, It.IsAny<CancellationToken>()));

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.UpdateAuction(auctionId, updateAuctionDto, _seller);

            //Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_UPDATE, response.Message);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(response.Errors);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task UpdateAuction_WhenAuctionNotUpdated_InternalServerError()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var updateAucitonDto = new UpdateAuctionDto();
            var auction = new Auction();
            auction.Seller = _seller;
            var auctionUpdated = new AuctionUpdated();

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            _unitOfWorkMock.Setup(u => u.Auction.UpdateAuction(auction));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(false);

            _updateAuctionValidatorMock.Setup(v => v.ValidateAsync(updateAucitonDto, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _mapperMock.Setup(m => m.Map(updateAucitonDto, auction.Item));

            _mapperMock.Setup(m => m.Map<AuctionUpdated>(auction))
                .Returns(auctionUpdated);

            _messagePublisherMock.Setup(mp => mp.PublishAsync(auctionUpdated, It.IsAny<CancellationToken>()));

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.UpdateAuction(auctionId, updateAucitonDto, _seller);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_FAILED, response.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Null(response.Errors);
            Assert.False(response.Data);
        }

        [Fact]
        public async Task DeleteAuction_WhenAuctionDoesNotExists_NotFound()
        {
            //Arrange
            var auctionId = Guid.NewGuid();

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync((Auction)null);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.DeleteAuction(auctionId, _seller);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_QUERY_EMPTY, response.Message);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.False(response.Data);
        }

        [Fact]
        public async Task DeleteAuction_WhenUserIsNotAuctionOwner_Forbidden()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction();
            auction.Seller = _anotherSeller;

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.DeleteAuction(auctionId, _seller);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_FORBID, response.Message);
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
            Assert.False(response.Data);
        }

        [Fact]
        public async Task DeleteAuction_WhenAuctionDeleted_Ok()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction() { Seller = _seller };
            var auctionDeleted = new AuctionDeleted() { Id = auctionId.ToString() };

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            _unitOfWorkMock.Setup(u => u.Auction.DeleteAuction(auction));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(true);

            _messagePublisherMock.Setup(mp => mp.PublishAsync(auctionDeleted, It.IsAny<CancellationToken>()));

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.DeleteAuction(auctionId, _seller);

            //Assert
            Assert.True(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_DELETE, response.Message);
            Assert.True(HttpStatusCode.OK == response.StatusCode);
            Assert.True(response.Data);
        }

        [Fact]
        public async Task DeleteAuction_WhenAuctionNotDeleted_InternalServerError()
        {
            //Arrange
            var auctionId = Guid.NewGuid();
            var auction = new Auction() { Seller = _seller };
            var auctionDeleted = new AuctionDeleted() { Id = auctionId.ToString() };

            _unitOfWorkMock.Setup(u => u.Auction.GetAuctionById(auctionId))
                .ReturnsAsync(auction);

            _unitOfWorkMock.Setup(u => u.Auction.DeleteAuction(auction));

            _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
                .ReturnsAsync(false);

            _messagePublisherMock.Setup(mp => mp.PublishAsync(auctionDeleted, It.IsAny<CancellationToken>()));

            AuctionApplication auctionApplicationService = CreateService();

            //Act
            var response = await auctionApplicationService.DeleteAuction(auctionId, _seller);

            //Assert
            Assert.False(response.IsSuccess);
            Assert.Equal(ReplyMessage.MESSAGE_FAILED, response.Message);
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.False(response.Data);
        }
    }
}
