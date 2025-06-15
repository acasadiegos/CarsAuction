using Application.Interfaces;
using Domain.Models;
using Infraestructure.Commons.Bases.Request;
using Infraestructure.Commons.Bases.Response;
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
        public async Task<ActionResult<BaseResponse<BaseEntityResponse<Item>>>> SearchItems([FromBody] BaseFiltersRequest filters)
        {
            return Ok(await _itemApplication.ListItems(filters));
        }
    }
}
