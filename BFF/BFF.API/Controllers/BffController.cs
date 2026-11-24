using BFF.Application.RemoteCall;
using Microsoft.AspNetCore.Mvc;

namespace BFF.API.Controllers;

[ApiController]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
[ProducesResponseType(StatusCodes.Status502BadGateway)]
[ProducesResponseType(StatusCodes.Status504GatewayTimeout)]
public abstract class BffController : ControllerBase
{
    protected IActionResult ProduceRemoteResponse(RemoteResponse response)
    {
        Response.StatusCode = response.StatusCode;
        return response.Body.Length == 0 ? new EmptyResult() : File(response.Body, response.ContentType ?? "application/octet-stream");
    }
}
