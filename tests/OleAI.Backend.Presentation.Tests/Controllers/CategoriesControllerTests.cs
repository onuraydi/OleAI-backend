using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.CreateCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.DeleteCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.UpdateCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryById;
using OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryList;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<IMediator> _mockMediator;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _mockMediator = new Mock<IMediator>();
        _controller = new CategoriesController();

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
    public async Task GetAll_ShouldSendQuery_AndReturnOk()
    {
        var response = new List<CategoryResponse> { new() { Id = Guid.NewGuid(), Name = "Bakliyat" } };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetCategoryListQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetAll();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);
        _mockMediator.Verify(m => m.Send(It.IsAny<GetCategoryListQuery>(), default), Times.Once);
    }

    [Fact]
    public async Task GetById_ShouldSendQueryWithId_AndReturnOk()
    {
        var categoryId = Guid.NewGuid();
        var response = new CategoryResponse { Id = categoryId, Name = "Sebze" };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<GetCategoryByIdQuery>(), default))
            .ReturnsAsync(response);

        var result = await _controller.GetById(categoryId);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.Value.Should().Be(response);

        _mockMediator.Verify(m => m.Send(It.Is<GetCategoryByIdQuery>(q => q.Id == categoryId), default), Times.Once);
    }

    [Fact]
    public async Task Add_ShouldSendCommand_AndReturnCreated()
    {
        var command = new CreateCategoryCommand
        {
            Request = new CreateCategoryRequest { Name = "Baharat" }
        };
        var response = new CreatedCategoryResponse { Id = Guid.NewGuid(), Name = "Baharat", Message = "ok" };

        _mockMediator
            .Setup(m => m.Send(command, default))
            .ReturnsAsync(response);

        var result = await _controller.Add(command);

        var createdResult = result as CreatedResult;
        createdResult.Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(command, default), Times.Once);
    }

    [Fact]
    public async Task Update_ShouldOverrideRouteIdInCommand_AndReturnOk()
    {
        var routeId = Guid.NewGuid();
        var command = new UpdateCategoryCommand
        {
            Request = new UpdateCategoryRequest { Id = Guid.NewGuid(), Name = "Yeni" }
        };

        _mockMediator
            .Setup(m => m.Send(It.IsAny<UpdateCategoryCommand>(), default))
            .ReturnsAsync("updated");

        var result = await _controller.Update(routeId, command);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<UpdateCategoryCommand>(c => c.Request.Id == routeId), default), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldSendCommandWithId_AndReturnOk()
    {
        var categoryId = Guid.NewGuid();

        _mockMediator
            .Setup(m => m.Send(It.IsAny<DeleteCategoryCommand>(), default))
            .ReturnsAsync("deleted");

        var result = await _controller.Delete(categoryId);

        (result as OkObjectResult).Should().NotBeNull();
        _mockMediator.Verify(m => m.Send(It.Is<DeleteCategoryCommand>(c => c.Id == categoryId), default), Times.Once);
    }
}
