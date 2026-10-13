using Asp.Versioning;
using Core.Abstraction.Paging;
using Core.Abstraction.Responses;
using Core.CQRS.Processing;
using Core.Host.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product.Abstraction.Requests.Products;
using Product.Abstraction.Responses.Products;
using Product.API.Authorization;
using Product.Application.Products;

namespace Product.API.Controllers.V1;

[ApiVersion("1.0")]
public class ProductsController(ICQRSProcessor processor) : BaseController
{
    [HttpGet("{id}")]
    [Authorize(Policy = ProductScopes.Read)]
    [ProducesResponseType(typeof(BaseResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponse<ProductDto>>> Find([FromRoute] string id, CancellationToken cancellationToken)
        => this.ProduceResponse(await processor.ProcessAsync(new FindProductQuery(id), cancellationToken));

    [HttpGet]
    [Authorize(Policy = ProductScopes.Read)]
    [ProducesResponseType(typeof(BaseResponse<IList<ListProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponse<IList<ListProductDto>>>> List(CancellationToken cancellationToken)
        => this.ProduceResponse(await processor.ProcessAsync(new ListProductsQuery(), cancellationToken));

    [HttpGet("search")]
    [Authorize(Policy = ProductScopes.Read)]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<ListProductDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponse<PagedResult<ListProductDto>>>> Search(
        [FromQuery] string? text,
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? orderByField,
        [FromQuery] OrderDirectionType? orderDirection,
        CancellationToken cancellationToken)
        => this.ProduceResponse(await processor.ProcessAsync(
            new SearchProductsQuery(text, page, pageSize, orderByField, orderDirection), cancellationToken));

    [HttpPost]
    [Authorize(Policy = ProductScopes.Write)]
    [ProducesResponseType(typeof(BaseResponse<ProductDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponse<ProductDto>>> Create([FromBody] CreateProductRequest request, CancellationToken cancellationToken)
        => this.ProduceCreatedResponse(await processor.ProcessAsync(
            new CreateProductCommand(request.Name, request.Sku, request.Description, request.Price, request.Stock), cancellationToken));

    [HttpPut("{id}")]
    [Authorize(Policy = ProductScopes.Write)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateDetails([FromRoute] string id, [FromBody] UpdateProductDetailsRequest request, CancellationToken cancellationToken)
    {
        await processor.ProcessAsync(new UpdateProductDetailsCommand(id, request.Name, request.Description), cancellationToken);
        return this.NoContent();
    }

    [HttpPatch("{id}/stock")]
    [Authorize(Policy = ProductScopes.Write)]
    [ProducesResponseType(typeof(BaseResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponse<ProductDto>>> UpdateStock([FromRoute] string id, [FromBody] UpdateProductStockRequest request, CancellationToken cancellationToken)
        => this.ProduceResponse(await processor.ProcessAsync(new UpdateProductStockCommand(id, request.Stock), cancellationToken));

    [HttpPatch("{id}/price")]
    [Authorize(Policy = ProductScopes.Write)]
    [ProducesResponseType(typeof(BaseResponse<ProductDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<BaseResponse<ProductDto>>> UpdatePrice([FromRoute] string id, [FromBody] UpdateProductPriceRequest request, CancellationToken cancellationToken)
        => this.ProduceResponse(await processor.ProcessAsync(new UpdateProductPriceCommand(id, request.Price), cancellationToken));

    [HttpDelete("{id}")]
    [Authorize(Policy = ProductScopes.Write)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> Delete([FromRoute] string id, CancellationToken cancellationToken)
    {
        await processor.ProcessAsync(new DeleteProductCommand(id), cancellationToken);
        return this.NoContent();
    }
}
