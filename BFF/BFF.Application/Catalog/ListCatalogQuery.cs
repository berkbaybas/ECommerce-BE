using System.Text.Json;
using BFF.Application.RemoteCall;
using Core.CQRS.Queries;

namespace BFF.Application.Catalog;

public sealed record ListCatalogQuery : IQuery<RemoteResponse>;

public sealed class ListCatalogQueryHandler(IProductRemoteCallService products, ICategoryRemoteCallService categories)
    : QueryHandler<ListCatalogQuery, RemoteResponse>
{
    public override async Task<RemoteResponse> Handle(ListCatalogQuery query, CancellationToken cancellationToken)
    {
        var productTask = products.List(cancellationToken);
        var categoryTask = categories.List(cancellationToken);
        await Task.WhenAll(productTask, categoryTask);
        var productResponse = await productTask;
        var categoryResponse = await categoryTask;
        if (productResponse.StatusCode is < 200 or >= 300 || categoryResponse.StatusCode is < 200 or >= 300)
            return Error();
        try
        {
            using var productJson = JsonDocument.Parse(productResponse.Body);
            using var categoryJson = JsonDocument.Parse(categoryResponse.Body);
            return new RemoteResponse(200, JsonSerializer.SerializeToUtf8Bytes(new
            {
                products = productJson.RootElement,
                categories = categoryJson.RootElement,
            }), "application/json");
        }
        catch (JsonException) { return Error(); }
    }

    private static RemoteResponse Error() => new(502, "{\"title\":\"A catalog service is unavailable or returned invalid data.\",\"status\":502}"u8.ToArray(), "application/problem+json");
}
