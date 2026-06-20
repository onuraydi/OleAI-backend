using System.Linq.Expressions;
using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Application.Features.ShoppingList.Queries.GetMyShoppingList;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Queries.GetMyShoppingList;

public class GetMyShoppingListQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnShoppingList()
    {
        var mockRepo = new Mock<IShoppingListItemRepository>();
        var mockMapper = new Mock<IMapper>();
        var handler = new GetMyShoppingListQueryHandler(mockRepo.Object, mockMapper.Object);

        var list = new List<ShoppingListItem>();
        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<ShoppingListItem>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new OleAI.Backend.Core.Repositories.Pagination.Paginate<ShoppingListItem> { Items = list });

        var response = new List<ShoppingListItemResponse>();
        mockMapper.Setup(m => m.Map<List<ShoppingListItemResponse>>(It.IsAny<List<ShoppingListItem>>())).Returns(response);

        var result = await handler.Handle(new GetMyShoppingListQuery(), CancellationToken.None);

        result.Should().NotBeNull();
    }
}
