using System.Security.Claims;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Application.Features.Recipes.Queries.GetMyComments;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Recipes.Queries.GetMyComments;

public class GetMyCommentsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnMyComments()
    {
        var mockRepo = new Mock<IRecipeCommentRepository>();
        var mockMapper = new Mock<IMapper>();
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();

        var userId = Guid.NewGuid();
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext { User = claimsPrincipal };

        mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        var comments = new Paginate<RecipeComment> { Items = new List<RecipeComment>() };
        mockRepo.Setup(r => r.GetListAsync(It.IsAny<QueryOptions<RecipeComment>>(), It.IsAny<CancellationToken>())).ReturnsAsync(comments);
        mockMapper.Setup(m => m.Map<Paginate<OleAI.Backend.Application.Features.Recipes.Dtos.Responses.MyCommentResponse>>(It.IsAny<Paginate<RecipeComment>>()))
            .Returns(new Paginate<OleAI.Backend.Application.Features.Recipes.Dtos.Responses.MyCommentResponse>());

        var handler = new GetMyCommentsQueryHandler(mockRepo.Object, mockMapper.Object, mockHttpContextAccessor.Object);

        var result = await handler.Handle(new GetMyCommentsQuery(), CancellationToken.None);

        result.Should().NotBeNull();
    }
}
