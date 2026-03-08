using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails
{
    public class ValidationProblemDetails : ProblemDetails
    {
        public ValidationProblemDetails(string detail)
        {
            Title = "Validation Error(s)";
            Detail = detail;
            Status = StatusCodes.Status400BadRequest;
            Type = "https://hatasayfasi";
        }
    }
}
