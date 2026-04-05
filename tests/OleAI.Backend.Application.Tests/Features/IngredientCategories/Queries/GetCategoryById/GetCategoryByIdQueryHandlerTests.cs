using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Queries.GetCategoryById;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryBusinessRules _rules;
    private readonly GetCategoryByIdQueryHandler _handler;

    public GetCategoryByIdQueryHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();

        _rules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
        _handler = new GetCategoryByIdQueryHandler(_mockCategoryRepository.Object, _mockMapper.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenCategoryNotFound_ShouldThrowNotFoundException()
    {
        // Arrange
        var query = new GetCategoryByIdQuery { Id = Guid.NewGuid() };

        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<NotFoundException>()
            .WithMessage(CategoryMessages.CategoryNotFoundMessage);
    }

    [Fact]
    public async Task Handle_WhenCategoryExists_ShouldMapAndReturnCategoryResponse()
    {
        // Arrange
        var category = new Category(Guid.NewGuid(), "Süt Ürünleri");
        var query = new GetCategoryByIdQuery { Id = category.Id };

        var mappedResponse = new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name
        };

        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(category);

        _mockMapper
            .Setup(m => m.Map<CategoryResponse>(category))
            .Returns(mappedResponse);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(category.Id);
        result.Name.Should().Be("Süt Ürünleri");

        _mockMapper.Verify(m => m.Map<CategoryResponse>(category), Times.Once);
    }
}
