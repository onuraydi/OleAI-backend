using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Pantry.Dtos;
using OleAI.Backend.Application.Features.Pantry.Dtos.Responses;
using OleAI.Backend.Application.Features.Pantry.Queries.GetMyPantry;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.Queries;

public class GetMyPantryQueryHandlerTests
{
    private readonly Mock<IUserIngredientRepository> _mockUserIngredientRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly GetMyPantryQueryHandler _handler;

    public GetMyPantryQueryHandlerTests()
    {
        _mockUserIngredientRepository = new Mock<IUserIngredientRepository>();
        _mockMapper = new Mock<IMapper>();

        _handler = new GetMyPantryQueryHandler(_mockUserIngredientRepository.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnMappedPantryList_When_UserHasIngredients()
    {
        // Arrange (Hazırlık)
        var query = new GetMyPantryQuery { UserId = Guid.NewGuid() };
        var masterIngredientId = Guid.NewGuid();

        // 1. Veritabanından dönecek (Mocklanmış) entity listesini hazırlıyoruz
        var userIngredients = new List<UserIngredient>
        {
            UserIngredient.CreateStandard(Guid.NewGuid(), query.UserId, masterIngredientId, 500, MeasurementUnit.Gram)
        };

        // GetListAsync metodu IPaginate döndüğü için sahte bir Paginate objesi oluşturuyoruz
        var paginateResult = new Paginate<UserIngredient> { Items = userIngredients };

        // Repository'nin GetListAsync metodunu mockluyoruz
        _mockUserIngredientRepository.Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<UserIngredient>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginateResult);

        // 2. AutoMapper'dan dönecek sahte DTO listesini hazırlıyoruz
        var mappedDtos = new List<PantryItemResponse>
        {
            new PantryItemResponse { MasterIngredientId = masterIngredientId, Name = "Domates", Quantity = 500 }
        };

        _mockMapper.Setup(m => m.Map<List<PantryItemResponse>>(userIngredients))
            .Returns(mappedDtos);

        // Act (Eylem)
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert (Doğrulama)
        result.Should().NotBeNull();
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Domates");
        result.First().Quantity.Should().Be(500);

        // Repository'nin ilgili metodu 1 kez çağırıldı mı? (Verify)
        _mockUserIngredientRepository.Verify(r => r.GetListAsync(
            It.IsAny<QueryOptions<UserIngredient>>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnEmptyList_When_PantryIsEmpty()
    {
        // Arrange
        var query = new GetMyPantryQuery { UserId = Guid.NewGuid() };

        var emptyPaginateResult = new Paginate<UserIngredient> { Items = new List<UserIngredient>() };

        _mockUserIngredientRepository.Setup(r => r.GetListAsync(
                It.IsAny<QueryOptions<UserIngredient>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(emptyPaginateResult);

        _mockMapper.Setup(m => m.Map<List<PantryItemResponse>>(It.IsAny<IList<UserIngredient>>()))
            .Returns(new List<PantryItemResponse>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }
}
