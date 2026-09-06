using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CKN.Sdk.Core.Exceptions;
using FluentValidation;
using MediatR;

namespace CKN.Sdk.Infrastructure.CQRS.Behaviors;

/// <summary>
/// Pipeline behavior to automatically validate MediatR requests using FluentValidation.
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Count != 0)
            {
                var errors = failures
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

                throw new CKN.Sdk.Core.Exceptions.ValidationException(errors);
            }
        }

        return await next();
    }
}
