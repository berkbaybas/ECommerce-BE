using Asp.Versioning;
using BFF.Application.Categories;
using global::Category.Abstraction;
using Core.Abstraction.Responses;
using Core.CQRS.Processing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers.Category.V1;

[ApiVersion("1.0")]
public class CategoriesController(ICQRSProcessor processor) : CategoryBaseController
{
    [HttpGet, Authorize(Policy = "categories.read")]
    [ProducesResponseType(typeof(BaseResponse<IList<CategoryDto>>), 200)]
    public async Task<IActionResult> List(CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new ListCategoriesQuery(), ct));
    [HttpGet("{id}"), Authorize(Policy = "categories.read")]
    [ProducesResponseType(typeof(BaseResponse<CategoryDto>), 200), ProducesResponseType(404)]
    public async Task<IActionResult> Find(string id, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new FindCategoryQuery(id), ct));
    [HttpPost, Authorize(Policy = "categories.write")]
    [ProducesResponseType(typeof(BaseResponse<CategoryDto>), 201), ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CategoryRequest request, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new CreateCategoryCommand(request), ct));
    [HttpPut("{id}"), Authorize(Policy = "categories.write")]
    [ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(404)]
    public async Task<IActionResult> Update(string id, [FromBody] CategoryRequest request, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new UpdateCategoryCommand(id, request), ct));
    [HttpDelete("{id}"), Authorize(Policy = "categories.write")]
    [ProducesResponseType(204), ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new DeleteCategoryCommand(id), ct));
}
