using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.CreateIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.DeleteIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientAliases.Dtos.Responses;
using OleAI.Backend.Application.Features.IngredientAliases.Queries.GetAliasesByIngredientId;
using OleAI.Backend.Application.Features.IngredientAliases.Queries.GetIngredientAliasList;
using OleAI.Backend.Application.Features.Ingredients.Commands.CreateIngredient;
using OleAI.Backend.Application.Features.Ingredients.Commands.DeleteIngredient;
using OleAI.Backend.Application.Features.Ingredients.Commands.UpdateIngredient;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Requests;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientById;
using OleAI.Backend.Application.Features.Ingredients.Queries.GetIngredientList;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Enums;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class IngredientsControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly IngredientsController _controller;

    public IngredientsControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new IngredientsController();

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
    public async Task GetList_Ingredients_ShouldSendQuery_AndReturnOk()
    {
        var query = new GetIngredientListQuery { Page = 1, Size = 10 };
        var response = new Paginate<IngredientResponse> { Items = new List<IngredientResponse>() };

        _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(response);

        var result = await _controller.GetList(query);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(query, default), Times.Once);
    }

    [Fact]
    public async Task Add_ShouldSendCommand_AndReturnCreated()
    {
        var command = new CreateIngredientCommand
        {
            Request = new CreateIngredientRequest { Name = "Tuz", CategoryId = Guid.NewGuid(), UnitFamily = MeasurementCategory.Weight }
        };
        var response = new CreatedIngredientResponse { Id = Guid.NewGuid(), Name = "Tuz" };

        _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync(response);

        var result = await _controller.Add(command);

        (result as CreatedResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendCommandWithId_AndReturnOk()
    {
        var id = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteIngredientCommand>(), default)).ReturnsAsync("deleted");

        var result = await _controller.Delete(id);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<DeleteIngredientCommand>(c => c.Id == id), default), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldOverrideRouteId_AndReturnOk()
    {
        var routeId = Guid.NewGuid();
        var command = new UpdateIngredientCommand
        {
            Request = new UpdateIngredientRequest { Id = Guid.NewGuid(), Name = "Yeni", CategoryId = Guid.NewGuid(), UnitFamily = MeasurementCategory.Count }
        };

        _mockMediator.Setup(m => m.Send(It.IsAny<UpdateIngredientCommand>(), default)).ReturnsAsync("updated");

        var result = await _controller.Update(routeId, command);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<UpdateIngredientCommand>(c => c.Request.Id == routeId), default), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldSendQueryWithId_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var response = new IngredientResponse { Id = id, Name = "Tuz" };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetIngredientByIdQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetById(id);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<GetIngredientByIdQuery>(q => q.Id == id), default), Times.Once);
    }

    [Fact]
    public async Task GetAliases_ShouldSendQueryWithIngredientId_AndReturnOk()
    {
        var ingredientId = Guid.NewGuid();
        var response = new List<IngredientAliasResponse>();
        _mockMediator.Setup(m => m.Send(It.IsAny<GetAliasesByIngredientIdQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetAliases(ingredientId);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<GetAliasesByIngredientIdQuery>(q => q.IngredientId == ingredientId), default), Times.Once);
    }

    [Fact]
    public async Task GetList_Aliases_ShouldSendQuery_AndReturnOk()
    {
        var query = new GetIngredientAliasListQuery();
        var response = new List<IngredientAliasResponse>();

        _mockMediator.Setup(m => m.Send(query, default)).ReturnsAsync(response);

        var result = await _controller.GetList(query);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(query, default), Times.Once);
    }

    [Fact]
    public async Task GetByIdAlias_ShouldSendIngredientByIdQuery_AndReturnOk()
    {
        var id = Guid.NewGuid();
        var response = new IngredientResponse { Id = id, Name = "Tuz" };
        _mockMediator.Setup(m => m.Send(It.IsAny<GetIngredientByIdQuery>(), default)).ReturnsAsync(response);

        var result = await _controller.GetByIdAlias(id);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<GetIngredientByIdQuery>(q => q.Id == id), default), Times.Once);
    }

    [Fact]
    public async Task AddAlias_ShouldBuildCommandFromRouteAndBody_AndReturnCreated()
    {
        var ingredientId = Guid.NewGuid();
        const string aliasName = "Domat";
        var response = new CreatedIngredientAliasResponse { IngredientId = ingredientId, AliasName = aliasName };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<CreateIngredientAliasCommand>(), default))
            .ReturnsAsync(response);

        var result = await _controller.AddAlias(ingredientId, aliasName);

        (result as CreatedResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(
            It.Is<CreateIngredientAliasCommand>(c => c.Request.IngredientId == ingredientId && c.Request.AliasName == aliasName),
            default), Times.Once);
    }

    [Fact]
    public async Task DeleteAlias_ShouldSendCommandWithAliasId_AndReturnOk()
    {
        var aliasId = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<DeleteIngredientAliasCommand>(), default)).ReturnsAsync("deleted");

        var result = await _controller.DeleteAlias(aliasId);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<DeleteIngredientAliasCommand>(c => c.Id == aliasId), default), Times.Once);
    }

    [Fact]
    public async Task RestoreAlias_ShouldSendCommandWithAliasId_AndReturnOk()
    {
        var aliasId = Guid.NewGuid();
        _mockMediator.Setup(m => m.Send(It.IsAny<OleAI.Backend.Application.Features.IngredientAliases.Commands.RestoreIngredientAlias.RestoreIngredientAliasCommand>(), default)).ReturnsAsync("restored");

        var result = await _controller.RestoreAlias(aliasId);

        (result as OkObjectResult).Should().NotBeNull();
    }
}
