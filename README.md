# ECommerce-BE

.NET 8 e-commerce Product service — Clean Architecture + CQRS, MongoDB (NoSQL) persistence, SignalR (WebSocket) real-time stock/price broadcasts, and OAuth 2.0 / OpenID Connect resource-server security (Keycloak for local dev).

## Run locally

```bash
# Everything in Docker: MongoDB + Keycloak (realm "ecommerce" auto-imported) + Product API
docker compose up -d --build
```

Alternatively run only the infrastructure in Docker and the API on the host:

```bash
docker compose up -d mongo keycloak
dotnet run --project Services/Product/Product.API
```

- Swagger UI: `http://localhost:5018/swagger` (Authorize → `oidc` → log in as `demo` / `demo`)
- SignalR hub: `ws://localhost:5018/hubs/products` (requires a token with the `products.read` scope)
- Live demo: `http://localhost:5018/demo/index.html`
- Keycloak admin: `http://localhost:8080`

If you already ran Keycloak with an older volume, remove it (`docker compose down -v`) so the realm is re-imported.

## Security

Every REST endpoint and the SignalR hub require a JWT access token issued by the OIDC provider:

| Scope | Grants |
|-------|--------|
| `products.read` | GET endpoints, connecting to the SignalR hub |
| `products.write` | POST / PUT / PATCH / DELETE |

Get a machine token (client credentials):

```bash
curl -X POST http://localhost:8080/realms/ecommerce/protocol/openid-connect/token \
  -d "grant_type=client_credentials&client_id=ecommerce-client&client_secret=ecommerce-client-secret"
```

Browsers cannot set an `Authorization` header on a WebSocket handshake, so SignalR clients pass the token as `?access_token=` (accepted only on `/hubs/*`).

## Try it

```bash
TOKEN=<access_token>

# Create a product
curl -X POST http://localhost:5018/api/v1/products \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"name":"Wireless Mouse","sku":"WM-001","price":19.99,"stock":50}'

# Update stock (broadcasts productStockUpdated over SignalR)
curl -X PATCH http://localhost:5018/api/v1/products/{id}/stock \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"stock":10}'
```

Open the live demo page, log in, then update a product's stock/price from Swagger — the event appears in the page's log. Call `SubscribeToProduct(productId)` to receive only that product's events, or stay unsubscribed to receive the full-catalog broadcast.


<img width="1418" height="646" alt="image" src="https://github.com/user-attachments/assets/889b2787-90e2-4bd1-8832-b5c0d3e8a7aa" />
<img width="1418" height="842" alt="image" src="https://github.com/user-attachments/assets/4e1d7af9-4b62-411f-a495-27d872ff1a33" />
<img width="1161" height="681" alt="image" src="https://github.com/user-attachments/assets/c5fe9eef-2b69-4818-be3c-d62dbe30af96" />
<img width="884" height="456" alt="image" src="https://github.com/user-attachments/assets/da05686b-a136-44d6-b8f6-adc1bba9c400" />


