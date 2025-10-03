using Application.Commons.Base.Response;
using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

    [HttpPost("search")]
    public async Task<ActionResult<BaseResponse<BaseEntityResponse<AuctionDto>>>> GetAll([FromBody] AuctionFiltersDto filters)
    {
        var response = await _auctionApplication.ListAuctions(filters);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaseResponse<AuctionDto>>> GetById(Guid id)
    {
        var response = await _auctionApplication.GetAuctionById(id);
        return StatusCode((int)response.StatusCode, response);
    }

    [Authorize]
    [HttpPost("create")]
    public async Task<ActionResult<BaseCreatedResponse<bool>>> Create([FromBody] CreateAuctionDto createAuctionDto)
    {
        var response = await _auctionApplication.CreateAuction(createAuctionDto, User.Identity.Name);

        if(response.StatusCode == HttpStatusCode.Created)
        {
            return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
        }

        return StatusCode((int)response.StatusCode, response);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<BaseResponse<bool>>> Update(Guid id, UpdateAuctionDto updateAuctionDto)
    {
        var response = await _auctionApplication.UpdateAuction(id, updateAuctionDto, User.Identity.Name);
        return StatusCode((int)response.StatusCode, response);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<BaseResponse<bool>>> DeleteAuction(Guid id)
    {
        var response = await _auctionApplication.DeleteAuction(id, User.Identity.Name);
        return StatusCode((int)response.StatusCode, response);
    }
}
