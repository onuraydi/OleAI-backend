using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.RecipeCategory;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Commands.Tag;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Dtos;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetCategories;
using OleAI.Backend.Application.Features.RecipeTaxonomy.Queries.GetTags;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers
{
    public class RecipeTaxonomyControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly RecipeTaxonomyController _controller;

        public RecipeTaxonomyControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new RecipeTaxonomyController();

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
        public async Task GetCategories_ShouldReturnOk()
        {
            var response = new List<RecipeCategoryDto>();
            _mockMediator.Setup(m => m.Send(It.IsAny<GetRecipeCategoriesQuery>(), default)).ReturnsAsync(response);

            var result = await _controller.GetCategories();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task AddCategory_ShouldReturnOk()
        {
            var command = new CreateRecipeCategoryCommand();
            var response = "Category created";
            _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync(response);

            var result = await _controller.AddCategory(command);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnOk()
        {
            var command = new UpdateRecipeCategoryCommand();
            var response = "Category updated";
            _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync(response);

            var result = await _controller.UpdateCategory(command);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnOk()
        {
            var response = "Category deleted";
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteRecipeCategoryCommand>(), default)).ReturnsAsync(response);

            var result = await _controller.DeleteCategory(1);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task GetTags_ShouldReturnOk()
        {
            var response = new List<TagDto>();
            _mockMediator.Setup(m => m.Send(It.IsAny<GetTagsQuery>(), default)).ReturnsAsync(response);

            var result = await _controller.GetTags();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task AddTag_ShouldReturnOk()
        {
            var command = new CreateTagCommand();
            var response = "Tag created";
            _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync(response);

            var result = await _controller.AddTag(command);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task UpdateTag_ShouldReturnOk()
        {
            var command = new UpdateTagCommand();
            var response = "Tag updated";
            _mockMediator.Setup(m => m.Send(command, default)).ReturnsAsync(response);

            var result = await _controller.UpdateTag(command);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }

        [Fact]
        public async Task DeleteTag_ShouldReturnOk()
        {
            var response = "Tag deleted";
            _mockMediator.Setup(m => m.Send(It.IsAny<DeleteTagCommand>(), default)).ReturnsAsync(response);

            var result = await _controller.DeleteTag(1);

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }
    }
}
