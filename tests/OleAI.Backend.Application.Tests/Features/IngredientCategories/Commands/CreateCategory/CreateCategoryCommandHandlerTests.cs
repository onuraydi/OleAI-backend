using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.IngredientCategories.Commands.CreateCategory;
using OleAI.Backend.Application.Features.IngredientCategories.Constants;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Requests;
using OleAI.Backend.Application.Features.IngredientCategories.Dtos.Response;
using OleAI.Backend.Application.Features.IngredientCategories.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientCategories.Commands.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IIngredientRepository> _mockIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryBusinessRules _rules;
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockIngredientRepository = new Mock<IIngredientRepository>();
        _mockMapper = new Mock<IMapper>();

        _rules = new CategoryBusinessRules(_mockCategoryRepository.Object, _mockIngredientRepository.Object);
        _handler = new CreateCategoryCommandHandler(_mockCategoryRepository.Object, _mockMapper.Object, _rules);
    }

    [Fact]
    public async Task Handle_WhenCategoryNameAlreadyExists_ShouldThrowBusinessException()
    {
        // Arrange
        var command = new CreateCategoryCommand
        {
            Request = new CreateCategoryRequest { Name = "Bakliyat" }
        };

        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Category(Guid.NewGuid(), "Bakliyat"));

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage(CategoryMessages.CategoryAlreadyExistsMessage);
    }

    [Fact]
    public async Task Handle_WhenRequestIsValid_ShouldCreateCategoryAndReturnResponse()
    {
        // Arrange
        var command = new CreateCategoryCommand
        {
            Request = new CreateCategoryRequest { Name = "Baharat" }
        };

        var mappedCategory = new Category(Guid.NewGuid(), "Baharat");
        var createdCategory = mappedCategory;
        var mappedResponse = new CreatedCategoryResponse { Id = createdCategory.Id, Name = createdCategory.Name };

        _mockCategoryRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<Category, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category?)null);

        _mockMapper
            .Setup(m => m.Map<Category>(command.Request))
            .Returns(mappedCategory);

        _mockCategoryRepository
            .Setup(r => r.AddAsync(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category c, CancellationToken _) => c);

        _mockMapper
            .Setup(m => m.Map<CreatedCategoryResponse>(It.IsAny<Category>()))
            .Returns(mappedResponse);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Baharat");
        result.Message.Should().Be(CategoryMessages.CreatedCategoryMessage);

        _mockCategoryRepository.Verify(r => r.AddAsync(
            It.Is<Category>(c => c.Name == "Baharat" && c.Id != Guid.Empty),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
