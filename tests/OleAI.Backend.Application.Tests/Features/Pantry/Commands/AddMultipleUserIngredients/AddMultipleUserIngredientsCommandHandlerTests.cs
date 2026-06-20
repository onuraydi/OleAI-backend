using FluentAssertions;
using MediatR;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Commands.AddMultipleUserIngredients;
using OleAI.Backend.Application.Features.Pantry.Commands.AddUserIngredient;
using OleAI.Backend.Application.Features.Pantry.Dtos.Requests;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Commands.AddMultipleUserIngredients;

public class AddMultipleUserIngredientsCommandHandlerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly AddMultipleUserIngredientsCommandHandler _handler;

    public AddMultipleUserIngredientsCommandHandlerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _handler = new AddMultipleUserIngredientsCommandHandler(_mockMediator.Object);
    }

    [Fact]
    public async Task Handle_ShouldDelegateToMediatorForEachItem()
    {
        // Arrange
        var request = new AddMultipleUserIngredientsCommand
        {
            Request = new AddMultipleUserIngredientsRequest
            {
                Ingredients = new List<AddUserIngredientRequest>
                {
                    new AddUserIngredientRequest { IngredientId = Guid.NewGuid() },
                    new AddUserIngredientRequest { IngredientId = Guid.NewGuid() }
                }
            }
        };

        var response = new AddedUserIngredientResponse { Id = Guid.NewGuid() };
        _mockMediator
            .Setup(m => m.Send(It.IsAny<AddUserIngredientCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        _mockMediator.Verify(m => m.Send(It.IsAny<AddUserIngredientCommand>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
