using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.Users.Commands.UpdateUserDislikedIngredients;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new UsersController();

        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup(x => x.GetService(typeof(IMediator))).Returns(_mockMediator.Object);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider.Object
            }
        };
    }

    [Fact]
    public async Task UpdateDislikedIngredients_ShouldSendCommand_AndReturnOk()
    {
        var command = new UpdateUserDislikedIngredientsCommand
        {
            IngredientIds = new List<Guid> { Guid.NewGuid() }
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateUserDislikedIngredientsCommand>(), default))
            .ReturnsAsync("Sevilmeyen malzemeleriniz başarıyla güncellendi.");

        var result = await _controller.UpdateDislikedIngredients(command);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();

        // Object result validation
        var value = okResult!.Value;
        var messageProp = value?.GetType().GetProperty("Message")?.GetValue(value);
        messageProp.Should().Be("Sevilmeyen malzemeleriniz başarıyla güncellendi.");

        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task GetProfile_ShouldSendQuery_AndReturnOk()
    {
        var response = new OleAI.Backend.Application.Features.Users.Dtos.Responses.UserProfileResponse();
        _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Users.Queries.GetUserProfile.GetUserProfileQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetProfile();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);
    }

    [Fact]
    public async Task UpdateProfile_ShouldSendCommand_AndReturnOk()
    {
        var command = new OleAI.Backend.Application.Features.Users.Commands.UpdateUserProfile.UpdateUserProfileCommand();
        _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Users.Commands.UpdateUserProfile.UpdateUserProfileCommand>(), default)).ReturnsAsync("Profile updated");

        var result = await _controller.UpdateProfile(command);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateDietaryPreferences_ShouldSendCommand_AndReturnOk()
    {
        var command = new OleAI.Backend.Application.Features.Users.Commands.UpdateUserPreferences.UpdateUserPreferencesCommand();
        _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Users.Commands.UpdateUserPreferences.UpdateUserPreferencesCommand>(), default)).ReturnsAsync("Preferences updated");

        var result = await _controller.UpdateDietaryPreferences(command);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAllergies_ShouldSendCommand_AndReturnOk()
    {
        var command = new OleAI.Backend.Application.Features.Users.Commands.UpdateUserAllergies.UpdateUserAllergiesCommand();
        _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Users.Commands.UpdateUserAllergies.UpdateUserAllergiesCommand>(), default)).ReturnsAsync("Allergies updated");

        var result = await _controller.UpdateAllergies(command);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRecommendations_ShouldSendQuery_AndReturnOk()
    {
        var query = new OleAI.Backend.Application.Features.Users.Queries.GetRecommendation.GetRecommendationQuery();
        var response = new List<OleAI.Backend.Application.Features.Users.Dtos.Responses.RecipeRecommendationDto>();
        _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.Users.Queries.GetRecommendation.GetRecommendationQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetRecommendations(query);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);
    }
}

