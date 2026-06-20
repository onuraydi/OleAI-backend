using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Commands.SyncAllToAi;
using OleAI.Backend.Application.Services.AiRecommendation;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Commands.SyncAllToAi;

public class SyncAllToAiCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldSyncToAi()
    {
        var mockRepo = new Mock<IRecipeRepository>();
        var mockAiService = new Mock<IAiRecommendationService>();
        var handler = new SyncAllToAiCommandHandler(mockRepo.Object, mockAiService.Object);

        var command = new SyncAllToAiCommand();
        var recipes = new Paginate<Recipe>
        {
            Items = new List<Recipe>(),
            TotalItems = 0
        };

        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<Recipe>>(), It.IsAny<CancellationToken>())).ReturnsAsync(recipes);

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().Be("0 recipes successfully synced with AI engine.");
    }
}
