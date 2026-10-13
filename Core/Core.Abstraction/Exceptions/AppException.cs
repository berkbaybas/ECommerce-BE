namespace Core.Abstraction.Exceptions;

public abstract class AppException : Exception
{
    protected AppException(string errorKey)
        : base(errorKey)
    {
        this.ErrorKey = errorKey;
    }

    public string ErrorKey { get; }
}
