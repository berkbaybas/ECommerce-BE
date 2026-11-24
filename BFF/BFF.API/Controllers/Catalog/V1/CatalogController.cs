using Asp.Versioning;
using BFF.Application.Catalog;
using Core.CQRS.Processing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers.Catalog.V1;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class CatalogController(ICQRSProcessor processor) : BffController
{
    [HttpGet, Authorize(Policy = "catalog.read")]
    [ProducesResponseType(typeof(CatalogResponse), 200), ProducesResponseType(401), ProducesResponseType(403), ProducesResponseType(502)]
    public async Task<IActionResult> List(CancellationToken ct) => ProduceRemoteResponse(await processor.ProcessAsync(new ListCatalogQuery(), ct));
}
