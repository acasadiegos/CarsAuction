using System;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Infraestructure.Commons.Bases.Response;

namespace Application.Mappers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Item, AuctionDto>();
        CreateMap<Auction, AuctionDto>()
            .IncludeMembers(x => x.Item)
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Item.Make))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Item.Model))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Item.Year))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Item.Color))
            .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.Item.Mileage))
            .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Item.ImageUrl));

        CreateMap<CreateAuctionDto, Auction>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionDto, Item>();
        CreateMap<UpdateAuctionDto, Item>();
        CreateMap<BaseEntityResponse<Auction>, BaseEntityResponse<AuctionDto>>()
        .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.Items));
    }
}
