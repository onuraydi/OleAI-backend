using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.ShoppingList.Commands.AddMultipleShoppingListItems;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Requests;
using OleAI.Backend.Application.Features.ShoppingList.Dtos.Responses;
using OleAI.Backend.Application.Features.ShoppingList.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.ShoppingList.Commands.AddMultipleShoppingListItems;

public class AddMultipleShoppingListItemsCommandHandlerTests
{
    private readonly Mock<IShoppingListItemRepository> _mockShoppingListItemRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly Mock<ICacheService> _mockCacheService;
    private readonly ShoppingListBusinessRules _businessRules;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IMediator> _mockMediator;
    private readonly AddMultipleShoppingListItemsCommandHandler _handler;

    public AddMultipleShoppingListItemsCommandHandlerTests()
    {
        _mockShoppingListItemRepository = new Mock<IShoppingListItemRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _mockCacheService = new Mock<ICacheService>();
        _businessRules = new ShoppingListBusinessRules(_mockShoppingListItemRepository.Object, _mockIngredientRepository.Object);
        _mockMapper = new Mock<IMapper>();
        _mockMediator = new Mock<IMediator>();

        _handler = new AddMultipleShoppingListItemsCommandHandler(
            _mockShoppingListItemRepository.Object,
            _mockIngredientRepository.Object,
            _mockIngredientRequestRepository.Object,
            _mockCacheService.Object,
            _businessRules,
            _mockMapper.Object,
            _mockMediator.Object);
    }

    [Fact]
    public async Task Handle_WhenNoIngredientOrRequestedName_ShouldThrowException()
    {
        // Arrange
        var command = new AddMultipleShoppingListItemsCommand
        {
            Request = new AddMultipleShoppingListItemsRequest
            {
                Items = new List<AddShoppingListItemRequest>
                {
                    new AddShoppingListItemRequest { IngredientId = null, RequestedName = "" }
                }
            }
        };

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>().WithMessage("Lütfen bir malzeme seçin veya yeni bir malzeme adı girin.");
    }
}
