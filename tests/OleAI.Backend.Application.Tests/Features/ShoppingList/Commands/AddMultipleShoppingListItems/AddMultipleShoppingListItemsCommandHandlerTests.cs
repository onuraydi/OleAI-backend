using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using OleAI.Backend.Application.Features.Ingredients.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Dtos;
using OleAI.Backend.Application.Features.ShoppingList.Commands.AddMultipleShoppingListItems;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Commands.AddMultipleShoppingListItems;

public class AddMultipleShoppingListItemsCommandHandlerTests
{
    [Fact]
    public async Task Handle_WithNoIngredientOrName_ShouldThrowException()
    {
        var mockRepo = new Mock<IShoppingListItemRepository>();
        var mockIngredientRepo = new Mock<IIngredientRepository>();
        var mockIngredientRequestRepo = new Mock<IIngredientRequestRepository>();
        var mockCache = new Mock<ICacheService>();
        var rules = new ShoppingListBusinessRules(mockRepo.Object, mockIngredientRepo.Object);
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();

        var handler = new AddMultipleShoppingListItemsCommandHandler(
            mockRepo.Object, mockIngredientRepo.Object, mockIngredientRequestRepo.Object,
            mockCache.Object, rules, mockMapper.Object, mockMediator.Object);

        var request = new AddMultipleShoppingListItemsCommand
        {
            Request = new AddMultipleShoppingListItemsRequest
            {
                UserId = Guid.NewGuid(),
                Items = new List<AddShoppingListItemRequest>
                {
                    new AddShoppingListItemRequest { IngredientId = null, RequestedName = "" }
                }
            }
        };

        var func = async () => await handler.Handle(request, CancellationToken.None);
        await func.Should().ThrowAsync<OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types.BusinessException>();
    }

    [Fact]
    public async Task Handle_WithValidRequest_ShouldReturnResponses()
    {
        var mockRepo = new Mock<IShoppingListItemRepository>();
        var mockIngredientRepo = new Mock<IIngredientRepository>();
        var mockIngredientRequestRepo = new Mock<IIngredientRequestRepository>();
        var mockCache = new Mock<ICacheService>();
        var rules = new ShoppingListBusinessRules(mockRepo.Object, mockIngredientRepo.Object);
        var mockMapper = new Mock<IMapper>();
        var mockMediator = new Mock<IMediator>();

        var handler = new AddMultipleShoppingListItemsCommandHandler(
            mockRepo.Object, mockIngredientRepo.Object, mockIngredientRequestRepo.Object,
            mockCache.Object, rules, mockMapper.Object, mockMediator.Object);

        var ingredientId = Guid.NewGuid();
        var request = new AddMultipleShoppingListItemsCommand
        {
            Request = new AddMultipleShoppingListItemsRequest
            {
                UserId = Guid.NewGuid(),
                Items = new List<AddShoppingListItemRequest>
                {
                    new AddShoppingListItemRequest { IngredientId = ingredientId, Unit = OleAI.Backend.Domain.Enums.MeasurementUnit.Gram, Quantity = 1 }
                }
            }
        };

        mockRepo.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<ShoppingListItem, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ShoppingListItem)null);

        var ingredient = new Ingredient(ingredientId, "Tomato", Guid.NewGuid(), OleAI.Backend.Domain.Enums.MeasurementCategory.Weight, null, OleAI.Backend.Domain.Enums.ExpirationMode.Auto, false);
        var cacheModel = new IngredientCacheModel { Id = ingredientId, Name = "Tomato", UnitFamily = OleAI.Backend.Domain.Enums.MeasurementCategory.Weight };

        mockIngredientRepo.Setup(r => r.GetAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Ingredient, bool>>>(), It.IsAny<Func<System.Linq.IQueryable<Ingredient>, Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Ingredient, object>>>(), It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ingredient);

        mockCache.Setup(c => c.GetAsync<IngredientCacheModel>(It.IsAny<string>())).ReturnsAsync(cacheModel);

        mockMapper.Setup(m => m.Map<List<ShoppingListItemResponse>>(It.IsAny<object>())).Returns(new List<ShoppingListItemResponse> { new ShoppingListItemResponse() });

        mockRepo.Setup(r => r.AddRangeAsync(It.IsAny<List<ShoppingListItem>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ShoppingListItem>());

        var result = await handler.Handle(request, CancellationToken.None);
        result.Should().NotBeNull();
    }
}
