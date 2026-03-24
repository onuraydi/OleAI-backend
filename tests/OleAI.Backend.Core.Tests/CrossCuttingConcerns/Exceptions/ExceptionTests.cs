using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using Xunit;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void AuthorizationException_Instantiate_ShouldSetMessage()
    {
        var msg = "Not authorized!";
        var ex = new AuthorizationException(msg);
        ex.Message.Should().Be(msg);
    }

    [Fact]
    public void AuthenticationProblemDetails_Instantiate_ShouldSetProperties()
    {
        var detail = "Auth Error";
        var type = "https://example.com/probs/auth";

        var problemDetails = new AuthenticationProblemDetails(detail)
        {
            Type = type
        };

        problemDetails.Title.Should().Be("Authentication Error");
        problemDetails.Detail.Should().Be(detail);
        problemDetails.Status.Should().Be(StatusCodes.Status403Forbidden);
        problemDetails.Type.Should().Be(type);
    }

    [Fact]
    public void AuthorizationProblemDetails_Instantiate_ShouldSetProperties()
    {
        var detail = "Authz Error";
        var type = "https://example.com/probs/authz";

        var problemDetails = new AuthorizationProblemDetails(detail)
        {
            Type = type
        };

        problemDetails.Title.Should().Be("Authorization error");
        problemDetails.Detail.Should().Be(detail);
        problemDetails.Status.Should().Be(StatusCodes.Status401Unauthorized);
        problemDetails.Type.Should().Be(type);
    }
}
