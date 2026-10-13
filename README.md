# ECommerce-BE

.NET 8 e-commerce Product service — Clean Architecture + CQRS, MongoDB persistence, SignalR real-time stock/price broadcasts.

See [CLAUDE.md](CLAUDE.md) for architecture, layering, and conventions.

## Run locally

```bash
# 1. Start MongoDB
docker compose up -d mongo

# 2. Run the API
dotnet run --project Services/Product/Product.API
```

Swagger UI: `http://localhost:<port>/swagger`
SignalR hub: `ws://localhost:<port>/hubs/products`

## Try it

```bash
# Create a product
curl -X POST http://localhost:<port>/api/v1/products \
  -H "Content-Type: application/json" \
  -d '{"name":"Wireless Mouse","sku":"WM-001","price":19.99,"stock":50}'

# Update stock (broadcasts productStockUpdated over SignalR)
curl -X PATCH http://localhost:<port>/api/v1/products/{id}/stock \
  -H "Content-Type: application/json" \
  -d '{"stock":10}'
```

Connect a SignalR client to `/hubs/products` and call `SubscribeToProduct(productId)` to receive that product's `productStockUpdated`/`productPriceUpdated` events, or stay unsubscribed to receive the full-catalog broadcast.
