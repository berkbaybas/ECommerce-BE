namespace Core.Abstraction.Responses;

public sealed class BaseResponse<T>
{
    public bool IsSuccess { get; private set; }

    public string? Message { get; private set; }

    public T? Data { get; private set; }

    public IReadOnlyCollection<string> Errors { get; private set; } = Array.Empty<string>();

    public static BaseResponse<T> Success(T? data, string? message = null)
        => new() { IsSuccess = true, Data = data, Message = message };

    public static BaseResponse<T> Fail(string message, IReadOnlyCollection<string>? errors = null)
        => new() { IsSuccess = false, Message = message, Errors = errors ?? Array.Empty<string>() };
}
