using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.IngredientAliases.Commands.RestoreIngredientAlias;
using OleAI.Backend.Application.Features.IngredientAliases.Constants;
using OleAI.Backend.Application.Features.IngredientAliases.Rules;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching;
using OleAI.Backend.Core.CrossCuttingConcerns.Exceptions.Types;
using OleAI.Backend.Core.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.IngredientAliases.Commands.RestoreIngredientAlias
{
    public class RestoreIngredientAliasCommandHandlerTests
    {
        private readonly Mock<IIngredientAliasRepository> _mockAliasRepository;
        private readonly Mock<IIngredientRepository> _mockIngredientRepository;
        private readonly Mock<ICacheService> _mockCacheService;
        private readonly IngredientAliasBusinessRules _rules;
        private readonly RestoreIngredientAliasCommandHandler _handler;

        public RestoreIngredientAliasCommandHandlerTests()
        {
            _mockAliasRepository = new Mock<IIngredientAliasRepository>();
            _mockIngredientRepository = new Mock<IIngredientRepository>();
            _mockCacheService = new Mock<ICacheService>();

            _rules = new IngredientAliasBusinessRules(_mockAliasRepository.Object, _mockIngredientRepository.Object);
            _handler = new RestoreIngredientAliasCommandHandler(_mockAliasRepository.Object, _rules, _mockCacheService.Object);
        }

        [Fact]
        public async Task Handle_WhenValidRequest_ShouldRestoreAliasAndReturnMessage()
        {
            // Arrange
            var command = new RestoreIngredientAliasCommand { Id = Guid.NewGuid() };
            var alias = new IngredientAlias(command.Id, Guid.NewGuid(), "DeletedAlias");
            var propertyInfo = typeof(Entity<Guid>).GetProperty("DeletedDate");
            propertyInfo?.SetValue(alias, DateTime.UtcNow);

            var paginatedResult = new Paginate<IngredientAlias>
            {
                Items = new List<IngredientAlias> { alias }
            };

            _mockAliasRepository.Setup(x => x.GetListAsync(
                    It.Is<QueryOptions<IngredientAlias>>(q => q.WithDeleted == true),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResult);

            _mockAliasRepository.Setup(x => x.UpdateAsync(It.IsAny<IngredientAlias>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(alias);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be("Eşanlamlı kelime başarıyla geri getirildi.");
            alias.DeletedDate.Should().BeNull();

            _mockAliasRepository.Verify(x => x.UpdateAsync(alias, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAliasNotFound_ShouldThrowNotFoundException()
        {
            // Arrange
            var command = new RestoreIngredientAliasCommand { Id = Guid.NewGuid() };

            var paginatedResult = new Paginate<IngredientAlias>
            {
                Items = new List<IngredientAlias>()
            };

            _mockAliasRepository.Setup(x => x.GetListAsync(
                    It.Is<QueryOptions<IngredientAlias>>(q => q.WithDeleted == true),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(paginatedResult);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<NotFoundException>().WithMessage(IngredientAliasMessages.AliasNotFoundMessage);
            _mockAliasRepository.Verify(x => x.UpdateAsync(It.IsAny<IngredientAlias>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
