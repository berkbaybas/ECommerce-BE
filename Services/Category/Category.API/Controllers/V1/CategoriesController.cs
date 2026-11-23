using Asp.Versioning;
using Core.Abstraction.Responses;
using Core.CQRS.Processing;
using Core.Host.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Category.Abstraction;
using Category.API.Authorization;
using Category.Application.Categories;
namespace Category.API.Controllers.V1;
[ApiVersion("1.0")]
public class CategoriesController(ICQRSProcessor processor) : BaseController
{
    [HttpGet("{id}")]
    [Authorize(Policy = CategoryScopes.Read)]
    [ProducesResponseType(typeof(BaseResponse<CategoryDto>), 200)]
    [ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<ActionResult<BaseResponse<CategoryDto>>> Find(string id, CancellationToken ct)
        => this.ProduceResponse(await processor.ProcessAsync(new FindCategoryQuery(id), ct));
    [HttpGet]
    [Authorize(Policy = CategoryScopes.Read)]
    [ProducesResponseType(typeof(BaseResponse<IList<CategoryDto>>), 200)]
    [ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(500)]
    public async Task<ActionResult<BaseResponse<IList<CategoryDto>>>> List(CancellationToken ct)
        => this.ProduceResponse(await processor.ProcessAsync(new ListCategoriesQuery(), ct));
    [HttpPost]
    [Authorize(Policy = CategoryScopes.Write)]
    [ProducesResponseType(typeof(BaseResponse<CategoryDto>), 201)]
    [ProducesResponseType(400), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(500)]
    public async Task<ActionResult<BaseResponse<CategoryDto>>> Create([FromBody] CategoryRequest request, CancellationToken ct)
        => this.ProduceCreatedResponse(await processor.ProcessAsync(new CreateCategoryCommand(request.Name, request.Description), ct));
    [HttpPut("{id}")]
    [Authorize(Policy = CategoryScopes.Write)]
    [ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<ActionResult> Update(string id, [FromBody] CategoryRequest request, CancellationToken ct)
    { await processor.ProcessAsync(new UpdateCategoryCommand(id, request.Name, request.Description), ct); return this.NoContent(); }
    [HttpDelete("{id}")]
    [Authorize(Policy = CategoryScopes.Write)]
    [ProducesResponseType(204), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(404), ProducesResponseType(500)]
    public async Task<ActionResult> Delete(string id, CancellationToken ct)
    { await processor.ProcessAsync(new DeleteCategoryCommand(id), ct); return this.NoContent(); }
}
