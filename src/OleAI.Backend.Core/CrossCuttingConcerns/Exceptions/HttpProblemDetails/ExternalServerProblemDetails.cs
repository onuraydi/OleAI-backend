using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails
{
    public class ExternalServerProblemDetails : ProblemDetails
    {
        public ExternalServerProblemDetails(string detail)
        {
            Title = "External Service Error";
            Detail = detail;
            Status = StatusCodes.Status502BadGateway;
            Type = "https://hatasayfasi";
        }
    }
}
