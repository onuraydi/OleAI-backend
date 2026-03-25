using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails
{
    public class AuthenticationProblemDetails : ProblemDetails
    {
        public AuthenticationProblemDetails(string detail)
        {
            Title = "Authentication Error";
            Detail = detail;
            Status = StatusCodes.Status403Forbidden;
            Type = "https://hatasayfasi";
        }
    }
}
