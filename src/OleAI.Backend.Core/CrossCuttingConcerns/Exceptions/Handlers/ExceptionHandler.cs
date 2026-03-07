using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Handlers
{
    public abstract class ExceptionHandler
    {
        public Task HandleExceptionAsync(Exception exception)
        {
            if (exception is AuthenticationException authenticationException)
                return HandleException(authenticationException);
            if (exception is AuthorizationException authorizationException)
                return HandleException(authorizationException);
            if (exception is BusinessException businessException)
                return HandleException(businessException);
            if (exception is InternalException internalException)
                return HandleException(internalException);
            if (exception is ExternalException externalException)
                return HandleException(externalException);
            if (exception is NotFoundException notFoundException)
                return HandleException(notFoundException);
            if (exception is ValidationException validationException)
                return HandleException(validationException);

            return HandleException(exception);
        }

        public abstract Task HandleExcepiton(AuthenticationException authenticationException);
        public abstract Task HandleExcepiton(AuthorizationException authorizationException);
        public abstract Task HandleExcepiton(BusinessException businessException);
        public abstract Task HandleExcepiton(InternalException internalException);
        public abstract Task HandleExcepiton(ExternalException externalException);
        public abstract Task HandleExcepiton(NotFoundException notFoundException);
        public abstract Task HandleExcepiton(ValidationException validationException);
        public abstract Task HandleException(Exception exception);
    }
}
