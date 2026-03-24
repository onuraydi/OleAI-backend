using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using Xunit;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Exceptions;

public class ExceptionTests
{
    // ──────────────────────────────────────────────
    // Exception Types
    // ──────────────────────────────────────────────

    [Fact]
    public void AuthorizationException_Instantiate_ShouldSetMessage()
    {
        var msg = "Not authorized!";
        var ex = new AuthorizationException(msg);
        ex.Message.Should().Be(msg);
    }

    [Fact]
    public void BusinessException_Instantiate_ShouldSetMessage()
    {
        var msg = "Business rule violated.";
        var ex = new BusinessException(msg);
        ex.Message.Should().Be(msg);
    }

    [Fact]
    public void AuthenticationException_Instantiate_ShouldSetMessage()
    {
        var msg = "Authentication failed.";
        var ex = new AuthenticationException(msg);
        ex.Message.Should().Be(msg);
    }

    [Fact]
    public void NotFoundException_Instantiate_ShouldSetMessage()
    {
        var msg = "Entity not found.";
        var ex = new NotFoundException(msg);
        ex.Message.Should().Be(msg);
    }

    [Fact]
    public void ValidationException_Instantiate_ShouldSetMessage()
    {
        var msg = "Validation failed.";
        var ex = new ValidationException(msg);
        ex.Message.Should().Be(msg);
    }

    [Fact]
    public void ExternalException_Instantiate_ShouldSetMessage()
    {
        var msg = "External service error.";
        var ex = new ExternalException(msg);
        ex.Message.Should().Be(msg);
    }

    [Fact]
    public void InternalException_Instantiate_ShouldSetMessage()
    {
        var msg = "Internal server error.";
        var ex = new InternalException(msg);
        ex.Message.Should().Be(msg);
    }

    // ──────────────────────────────────────────────
    // HttpProblemDetails
    // ──────────────────────────────────────────────

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

    [Fact]
    public void BusinessProblemDetails_Instantiate_ShouldSetStatusAndTitle()
    {
        var detail = "Business rule failure.";
        var pd = new BusinessProblemDetails(detail);

        pd.Title.Should().Be("Business Rule Violation");
        pd.Detail.Should().Be(detail);
        pd.Status.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public void NotFoundProblemDetails_Instantiate_ShouldSetStatusAndTitle()
    {
        var detail = "Resource not found.";
        var pd = new NotFoundProblemDetails(detail);

        pd.Title.Should().Be("Not Found");
        pd.Detail.Should().Be(detail);
        pd.Status.Should().Be(StatusCodes.Status404NotFound);
    }
}
