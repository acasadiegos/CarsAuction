using Application.Commons.Bases.Request;
using Application.Commons.Bases.Response;
using Application.DTOs;
using Application.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace SearchService.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        IItemApplication _itemApplication;
        public SearchController(IItemApplication itemApplication)
        {
            _itemApplication = itemApplication;
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<Item>>>> SearchItems([FromBody] ItemFiltersDto filters)
        {
            var response = await _itemApplication.ListItems(filters);
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
