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

        public abstract Task HandleException(AuthenticationException authenticationException);
        public abstract Task HandleException(AuthorizationException authorizationException);
        public abstract Task HandleException(BusinessException businessException);
        public abstract Task HandleException(InternalException internalException);
        public abstract Task HandleException(ExternalException externalException);
        public abstract Task HandleException(NotFoundException notFoundException);
        public abstract Task HandleException(ValidationException validationException);
        public abstract Task HandleException(Exception exception);
    }
}
