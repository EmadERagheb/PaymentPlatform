namespace BuildingBlocks.Exceptions;

public sealed class ValidationException(IEnumerable<ValidationError> errors)
        : Exception(string.Join("; ", errors.Select(e => e.ErrorMessage)))
{
    public IEnumerable<ValidationError> Errors { get; } = errors;
}
