namespace Core.Abstraction.Exceptions;

public sealed class NotFoundException : AppException
{
    private NotFoundException(string errorKey)
        : base(errorKey)
    {
    }

    public static void Throw(string errorKey) => throw new NotFoundException(errorKey);

    public static void ThrowIfNull(object? value, string errorKey)
    {
        if (value is null)
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
