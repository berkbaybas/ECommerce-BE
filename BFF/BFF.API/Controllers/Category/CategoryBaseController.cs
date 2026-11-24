using Microsoft.AspNetCore.Mvc;
namespace BFF.API.Controllers.Category;
[Route("category/api/v{version:apiVersion}/[controller]")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class CategoryBaseController : BffController;
