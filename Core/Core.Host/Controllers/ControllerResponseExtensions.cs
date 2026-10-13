using Core.Abstraction.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Core.Host.Controllers;

public static class ControllerResponseExtensions
{
    public static ActionResult<BaseResponse<T>> ProduceResponse<T>(this ControllerBase controller, T data)
        => controller.Ok(BaseResponse<T>.Success(data));

    public static ActionResult<BaseResponse<T>> ProduceCreatedResponse<T>(this ControllerBase controller, T data)
        => controller.StatusCode(StatusCodes.Status201Created, BaseResponse<T>.Success(data));
}
