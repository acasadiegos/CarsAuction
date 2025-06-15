using System;
using Application.Commons.Base;
using Application.DTOs;
using Application.Interfaces;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;
using Microsoft.AspNetCore.Mvc;

namespace AuctionService.Controllers;

[ApiController]
[Route("api/auctions")]
public class AuctionsController : ControllerBase
{
    private readonly IAuctionApplication _auctionApplication;

    public AuctionsController(IAuctionApplication auctionApplication)
    {
        _auctionApplication = auctionApplication;
    }

    [HttpPost]
    public async Task<ActionResult<BaseResponse<BaseEntityResponse<AuctionDto>>>> GetAll([FromBody] BaseFiltersRequest filters)
    {
        var response = await _auctionApplication.ListAuctions(filters);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaseResponse<AuctionDto>>> GetById(Guid id)
    {
        var response = await _auctionApplication.GetAuctionById(id);
        return Ok(response);
    }

    [HttpPost("create")]
    public async Task<ActionResult<BaseResponse<bool>>> Create([FromBody] CreateAuctionDto createAuctionDto)
    {
        var response = await _auctionApplication.CreateAuction(createAuctionDto);
        return Ok(response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BaseResponse<bool>>> Update(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var response = await _auctionApplication.UpdateAuction(id, updateAuctionDto);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteAuction(Guid id)
    {
        var response = await _auctionApplication.DeleteAuction(id);
        return Ok(response);
    }
}
