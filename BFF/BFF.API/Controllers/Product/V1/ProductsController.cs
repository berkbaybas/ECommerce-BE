using Asp.Versioning;
using BFF.Application.Products;
using Core.Abstraction.Paging;
using Core.Abstraction.Responses;
using Core.CQRS.Processing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using global::Product.Abstraction.Requests.Products;
using global::Product.Abstraction.Responses.Products;

namespace BFF.API.Controllers.Product.V1;

[ApiVersion("1.0")]
public class ProductsController(ICQRSProcessor processor) : ProductBaseController
{
    [HttpGet, Authorize(Policy = "products.read")]
    [ProducesResponseType(typeof(BaseResponse<IList<ListProductDto>>), 200)]
    public async Task<IActionResult> List(CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new ListProductsQuery(), ct));

    [HttpGet("search"), Authorize(Policy = "products.read")]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<ListProductDto>>), 200)]
    public async Task<IActionResult> Search([FromQuery] string? text, [FromQuery] int? page, [FromQuery] int? pageSize, [FromQuery] string? orderByField, [FromQuery] OrderDirectionType? orderDirection, CancellationToken ct)
        => ProduceRemoteResponse(await processor.ProcessAsync(new SearchProductsQuery(Request.QueryString.Value ?? ""), ct));

    [HttpGet("{id}"), Authorize(Policy = "products.read")]
    [ProducesResponseType(typeof(BaseResponse<ProductDto>), 200), ProducesResponseType(404)]
    public async Task<IActionResult> Find(string id, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new FindProductQuery(id), ct));

    [HttpPost, Authorize(Policy = "products.write")]
    [ProducesResponseType(typeof(BaseResponse<ProductDto>), 201), ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateProductRequest request, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new CreateProductCommand(request), ct));

    [HttpPut("{id}"), Authorize(Policy = "products.write")]
    [ProducesResponseType(204), ProducesResponseType(400), ProducesResponseType(404)]
    public async Task<IActionResult> UpdateDetails(string id, [FromBody] UpdateProductDetailsRequest request, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new UpdateProductDetailsCommand(id, request), ct));

    [HttpPatch("{id}/stock"), Authorize(Policy = "products.write")]
    [ProducesResponseType(typeof(BaseResponse<ProductDto>), 200), ProducesResponseType(400), ProducesResponseType(404)]
    public async Task<IActionResult> UpdateStock(string id, [FromBody] UpdateProductStockRequest request, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new UpdateProductStockCommand(id, request), ct));

    [HttpPatch("{id}/price"), Authorize(Policy = "products.write")]
    [ProducesResponseType(typeof(BaseResponse<ProductDto>), 200), ProducesResponseType(400), ProducesResponseType(404)]
    public async Task<IActionResult> UpdatePrice(string id, [FromBody] UpdateProductPriceRequest request, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new UpdateProductPriceCommand(id, request), ct));

    [HttpDelete("{id}"), Authorize(Policy = "products.write")]
    [ProducesResponseType(204), ProducesResponseType(404)]
    public async Task<IActionResult> Delete(string id, CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new DeleteProductCommand(id), ct));
}
