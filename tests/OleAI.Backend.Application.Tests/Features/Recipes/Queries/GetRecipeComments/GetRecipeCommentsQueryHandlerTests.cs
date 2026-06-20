using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Queries.GetRecipeComments;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetRecipeComments;

public class GetRecipeCommentsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnRecipeComments()
    {
        var mockRepo = new Mock<IRecipeCommentRepository>();
        var mockMapper = new Mock<IMapper>();

        var comments = new Paginate<RecipeComment> { Items = new List<RecipeComment>() };
        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<RecipeComment>>(), It.IsAny<CancellationToken>())).ReturnsAsync(comments);
        mockMapper.Setup(m => m.Map<Paginate<OleAI.Backend.Application.Features.Recipes.Dtos.Responses.RecipeCommentResponse>>(It.IsAny<Paginate<RecipeComment>>()))
            .Returns(new Paginate<OleAI.Backend.Application.Features.Recipes.Dtos.Responses.RecipeCommentResponse> { Items = new List<OleAI.Backend.Application.Features.Recipes.Dtos.Responses.RecipeCommentResponse>() });

        var handler = new GetRecipeCommentsQueryHandler(mockRepo.Object, mockMapper.Object);

        var result = await handler.Handle(new GetRecipeCommentsQuery(), CancellationToken.None);

        result.Should().NotBeNull();
    }
}
