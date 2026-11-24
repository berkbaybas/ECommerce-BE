# ECommerce-BE

.NET 8 e-commerce backend with Product and Category microservices, a BFF, MongoDB,
Keycloak authentication and SignalR product updates.

## Run locally

```bash
docker compose up -d --build
```

The BFF is the public API entry point at `http://localhost:5020`. Product and
Category are available only on the Docker network; their ports are not published.
Keycloak remains accessible at `http://localhost:8080`, and MongoDB at port 27017.
The BFF Swagger UI is available at `http://localhost:5020/swagger`; use Authorize
to enter an access token before calling the endpoints.
Service-grouped routes follow the RayConnect BFF convention:
`/product/api/v1/products` and `/category/api/v1/categories`. The shorter
`/api/v1/products` and `/api/v1/categories` routes remain available as aliases.

| BFF route | Purpose | Required scope |
| --- | --- | --- |
| `/api/v1/products` | Product CRUD and search | `products.read` / `products.write` |
| `/api/v1/categories` | Category CRUD | `categories.read` / `categories.write` |
| `GET /api/v1/catalog` | Combined product and category lists | Both read scopes |
| `/hubs/products` | SignalR negotiation and transports | `products.read` |
| `/demo/index.html` | SignalR demo | Login through Keycloak |

REST requests retain their paths, query strings, bodies, status codes and response
bodies when forwarded to the corresponding service. The catalog endpoint returns
each service's response envelope under `products` or `categories`, and responds
with 502 when a service fails. The BFF accepts bearer tokens and forwards them to
the services; it does not implement cookie-based login or token storage.

The `ecommerce-client` machine client receives the BFF and service audiences and
the read/write scopes. To get a token and call the BFF:

```bash
curl -X POST http://localhost:8080/realms/ecommerce/protocol/openid-connect/token \
  -d "grant_type=client_credentials&client_id=ecommerce-client&client_secret=ecommerce-client-secret"

TOKEN=<access_token>
curl http://localhost:5020/api/v1/catalog -H "Authorization: Bearer $TOKEN"
curl -X POST http://localhost:5020/api/v1/products \
  -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
  -d '{"name":"Wireless Mouse","sku":"WM-001","price":19.99,"stock":50}'
```

The demo at `http://localhost:5020/demo/index.html` logs in as `demo` / `demo`
and connects to the SignalR hub through the BFF. Browser SignalR clients can pass
the access token as `?access_token=` on hub routes.

For a frontend, the public `bff-client` supports authorization code + PKCE with
`http://localhost:3000` as the development origin. Configure its Keycloak redirect
URIs and `Cors:AllowedOrigins` for the frontend's actual address. Existing
Keycloak realms must be updated or re-imported to include the BFF audience, client
and demo redirect URI. Recreating realm data removes existing realm data.

For host-based development, start infrastructure with `docker compose up -d mongo
keycloak`, then run `Services/Product/Product.API`, `Services/Category/Category.API`,
and `BFF/BFF.API` using `dotnet run --project <path>`. The BFF's default upstream
addresses are localhost ports 5018 and 5019. The individual service Swagger pages
are available on those ports when the services run directly on the host.

<img width="1418" height="646" alt="Product demo" src="https://github.com/user-attachments/assets/889b2787-90e2-4bd1-8832-b5c0d3e8a7aa" />
<img width="1418" height="842" alt="Product demo" src="https://github.com/user-attachments/assets/4e1d7af9-4b62-411f-a495-27d872ff1a33" />
<img width="1161" height="681" alt="Product demo" src="https://github.com/user-attachments/assets/c5fe9eef-2b69-4818-be3c-d62dbe30af96" />
<img width="884" height="456" alt="Product demo" src="https://github.com/user-attachments/assets/da05686b-a136-44d6-b8f6-adc1bba9c400" />
