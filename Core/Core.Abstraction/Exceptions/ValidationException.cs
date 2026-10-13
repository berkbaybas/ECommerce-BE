namespace Core.Abstraction.Exceptions;

public sealed class ValidationException : AppException
{
    public ValidationException(IReadOnlyCollection<string> errorKeys)
        : base(errorKeys.FirstOrDefault() ?? "Validation.Failed")
    {
        this.ErrorKeys = errorKeys;
    }

    public IReadOnlyCollection<string> ErrorKeys { get; }
}
