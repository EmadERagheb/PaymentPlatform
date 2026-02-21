using FluentValidation;
using ValidationException = BuildingBlocks.Exceptions.ValidationException;

namespace BuildingBlocks.Behaviors;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
   where TRequest : IRequest<TResponse>

{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next();
        }
        var context = new ValidationContext<TRequest>(request);
        var validationErrors = validators.Select(validators => validators.Validate(context))
                                         .Where(validatorResult => validatorResult.Errors.Any())
                                         .SelectMany(result => result.Errors)
                                         .Select(validationFailure => new ValidationError(validationFailure.PropertyName, validationFailure.ErrorMessage))
                                         .ToList();
        if (validationErrors.Any())
        {
            throw new ValidationException(validationErrors);
        }
        return await next(cancellationToken);
    }
}
