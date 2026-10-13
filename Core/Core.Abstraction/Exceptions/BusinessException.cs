namespace Core.Abstraction.Exceptions;

public sealed class BusinessException : AppException
{
    private BusinessException(string errorKey)
        : base(errorKey)
    {
    }

    public static void Throw(string errorKey) => throw new BusinessException(errorKey);

    public static void ThrowIfNull(object? value, string errorKey)
    {
        if (value is null)
        {
            Throw(errorKey);
        }
    }

    public static void ThrowIfNullOrWhiteSpace(string? value, string errorKey)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            Throw(errorKey);
        }
    }

    public static void ThrowIf(bool condition, string errorKey)
    {
        if (condition)
        {
            Throw(errorKey);
        }
    }
}
