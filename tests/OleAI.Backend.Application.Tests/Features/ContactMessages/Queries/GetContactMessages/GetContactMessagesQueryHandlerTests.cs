using AutoMapper;
using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.ContactMessages.Queries.GetContactMessages;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using Xunit;
using static OleAI.Backend.Application.Features.ContactMessages.Queries.GetContactMessages.GetContactMessagesQuery;

namespace OleAI.Backend.Application.Tests.Features.ContactMessages.Queries.GetContactMessages;

public class GetContactMessagesQueryHandlerTests
{
    private readonly Mock<IContactMessageRepository> _contactMessageRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetContactMessagesQueryHandler _handler;

    public GetContactMessagesQueryHandlerTests()
    {
        _contactMessageRepositoryMock = new Mock<IContactMessageRepository>();
        _mapperMock = new Mock<IMapper>();

        _handler = new GetContactMessagesQueryHandler(
            _contactMessageRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPaginatedContactMessages()
    {
        // Arrange
        var query = new GetContactMessagesQuery { Page = 0, Size = 10 };

        var contactMessages = new List<ContactMessage>
        {
            new ContactMessage { Id = Guid.NewGuid(), Name = "Alice", Email = "alice@example.com", Department = "Billing", Message = "Invoice issue", IsRead = false },
            new ContactMessage { Id = Guid.NewGuid(), Name = "Bob", Email = "bob@example.com", Department = "Tech", Message = "Login issue", IsRead = true }
        };

        var paginatedContactMessages = new Paginate<ContactMessage>
        {
            Items = contactMessages,
            PageNumber = 0,
            PageSize = 10,
            TotalItems = 2,
            TotalPages = 1
        };

        var expectedDtoList = new Paginate<GetContactMessageListItemDto>
        {
            Items = new List<GetContactMessageListItemDto>
            {
                new GetContactMessageListItemDto { Id = contactMessages[0].Id, Name = "Alice", Email = "alice@example.com", Department = "Billing", IsRead = false },
                new GetContactMessageListItemDto { Id = contactMessages[1].Id, Name = "Bob", Email = "bob@example.com", Department = "Tech", IsRead = true }
            },
            PageNumber = 0,
            PageSize = 10,
            TotalItems = 2,
            TotalPages = 1
        };

        _contactMessageRepositoryMock.Setup(x => x.GetListAsync(
                It.IsAny<QueryOptions<ContactMessage>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedContactMessages);

        _mapperMock.Setup(x => x.Map<Paginate<GetContactMessageListItemDto>>(paginatedContactMessages))
            .Returns(expectedDtoList);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
        result.PageNumber.Should().Be(0);
        result.PageSize.Should().Be(10);
        result.TotalItems.Should().Be(2);

        _contactMessageRepositoryMock.Verify(x => x.GetListAsync(
            It.Is<QueryOptions<ContactMessage>>(q => q.Index == query.Page && q.Size == query.Size),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
