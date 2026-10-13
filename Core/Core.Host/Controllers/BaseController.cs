using Microsoft.AspNetCore.Mvc;

namespace Core.Host.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseController : ControllerBase
{
}
