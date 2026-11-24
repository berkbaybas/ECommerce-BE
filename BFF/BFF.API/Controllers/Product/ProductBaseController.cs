using Microsoft.AspNetCore.Mvc;
namespace BFF.API.Controllers.Product;
[Route("product/api/v{version:apiVersion}/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class ProductBaseController : BffController;
