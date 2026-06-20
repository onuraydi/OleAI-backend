
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails
{
    public class BusinessProblemDetails : ProblemDetails
    {
        public string? ErrorCode { get; set; }

        public BusinessProblemDetails(string detail, string? errorCode = null)
        {
            Title = "Business Rule Violation";
            Detail = detail;
            Status = StatusCodes.Status400BadRequest;
            Type = "https://hatasayfasi";
            ErrorCode = errorCode;
        }
    }
}
