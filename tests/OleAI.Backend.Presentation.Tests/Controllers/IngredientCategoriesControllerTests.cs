using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryList;
using OleAI.Backend.Presentation.Controllers;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Controllers
{
    public class IngredientCategoriesControllerTests
    {
        private readonly Mock<IMediator> _mockMediator;
        private readonly IngredientCategoriesController _controller;

        public IngredientCategoriesControllerTests()
        {
            _mockMediator = new Mock<IMediator>();
            _controller = new IngredientCategoriesController();

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
            var response = new List<OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response.CategoryResponse>();
            _mockMediator.Setup(m => m.Send(It.IsAny<GetCategoryListQuery>(), default)).ReturnsAsync(response);

            var result = await _controller.GetAll();

            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().Be(response);
        }
    }
}
