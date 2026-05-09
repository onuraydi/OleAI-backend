using FluentValidation;
using MediatR;
using ValidationException = OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types.ValidationException;

namespace OleAI.Backend.Application.Behaviors
{
    /// <summary>
    /// MediatR pipeline'ında FluentValidation kurallarını otomatik olarak işleten davranış.
    /// Her request handler'ından önce çalışır; doğrulama hatası varsa ValidationException fırlatır.
    /// </summary>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .SelectMany(r => r.Errors)
                .Where(f => f is not null)
                .ToList();

            if (failures.Count > 0)
            {
                var errorMessages = string.Join(" | ",
                    failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}"));
                throw new ValidationException(errorMessages);
            }

            return await next();
        }
    }
}
