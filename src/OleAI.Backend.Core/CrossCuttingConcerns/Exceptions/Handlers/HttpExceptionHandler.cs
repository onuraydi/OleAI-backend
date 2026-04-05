using System.Text.Json;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Handlers
{
    public class HttpExceptionHandler : ExceptionHandler
    {
        private HttpResponse? _response;

        public HttpResponse Response
        {
            get => _response ?? throw new InvalidOperationException(nameof(_response));
            set => _response = value;
        }

        public override Task HandleException(AuthenticationException authenticationException)
        {
            Response.StatusCode = StatusCodes.Status403Forbidden;
            var detail = new AuthenticationProblemDetails(authenticationException.Message);
            return WriteAsJsonAsync(Response, detail);
        }

        public override Task HandleException(AuthorizationException authorizationException)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            var detail = new AuthorizationProblemDetails(authorizationException.Message);
            return WriteAsJsonAsync(Response, detail);
        }

        public override Task HandleException(BusinessException businessException)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            var detail = new BusinessProblemDetails(businessException.Message);
            return WriteAsJsonAsync(Response, detail);
        }

        public override Task HandleException(InternalException internalException)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            var detail = new InternalServerProblemDetails(internalException.Message);
            return WriteAsJsonAsync(Response, detail);
        }

        public override Task HandleException(ExternalException externalException)
        {
            Response.StatusCode = StatusCodes.Status502BadGateway;
            var detail = new ExternalServerProblemDetails(externalException.Message);
            return WriteAsJsonAsync(Response, detail);
        }

        public override Task HandleException(NotFoundException notFoundException)
        {
            Response.StatusCode = StatusCodes.Status404NotFound;
            var detail = new NotFoundProblemDetails(notFoundException.Message);
            return WriteAsJsonAsync(Response, detail);
        }

        public override Task HandleException(ValidationException validationException)
        {
            Response.StatusCode = StatusCodes.Status400BadRequest;
            var detail = new ValidationProblemDetails(validationException.Message);
            return WriteAsJsonAsync(Response, detail);
        }

        public override Task HandleException(Exception exception)
        {
            Response.StatusCode = StatusCodes.Status500InternalServerError;
            var detail = new InternalServerProblemDetails("Sunucu içi bir hata oluştu.");
            return WriteAsJsonAsync(Response, detail);
        }

        private static Task WriteAsJsonAsync<T>(HttpResponse response, T value)
        {
            response.ContentType = "application/json";
            return JsonSerializer.SerializeAsync(response.Body, value);
        }
    }
}
