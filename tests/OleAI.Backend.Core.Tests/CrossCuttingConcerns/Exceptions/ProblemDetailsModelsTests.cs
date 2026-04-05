using FluentAssertions;
using Microsoft.AspNetCore.Http;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.HttpProblemDetails;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Exceptions;

public class ProblemDetailsModelsTests
{
    [Fact]
    public void ValidationProblemDetails_ShouldSetDefaults()
    {
        var pd = new ValidationProblemDetails("validation failed");

        pd.Title.Should().Be("Validation Error(s)");
        pd.Detail.Should().Be("validation failed");
        pd.Status.Should().Be(StatusCodes.Status400BadRequest);
        pd.Type.Should().Be("https://hatasayfasi");
    }

    [Fact]
    public void InternalServerProblemDetails_ShouldSetDefaults()
    {
        var pd = new InternalServerProblemDetails("internal error");

        pd.Title.Should().Be("Internal Server Error");
        pd.Detail.Should().Be("internal error");
        pd.Status.Should().Be(StatusCodes.Status500InternalServerError);
        pd.Type.Should().Be("https://hatasayfasi");
    }

    [Fact]
    public void ExternalServerProblemDetails_ShouldSetDefaults()
    {
        var pd = new ExternalServerProblemDetails("gateway error");

        pd.Title.Should().Be("External Service Error");
        pd.Detail.Should().Be("gateway error");
        pd.Status.Should().Be(StatusCodes.Status502BadGateway);
        pd.Type.Should().Be("https://hatasayfasi");
    }
}
