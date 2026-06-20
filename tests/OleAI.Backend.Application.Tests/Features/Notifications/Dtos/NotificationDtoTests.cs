using FluentAssertions;
using OleAI.Backend.Application.Features.Notifications.Dtos;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Notifications.Dtos;

public class NotificationDtoTests
{
    [Fact]
    public void Properties_ShouldBeGettableAndSettable()
    {
        var dto = new NotificationDto();
        var id = System.Guid.NewGuid();
        var date = System.DateTime.UtcNow;

        dto.Id = id;
        dto.Title = "Title";
        dto.Message = "Message";
        dto.Type = NotificationType.System;
        dto.RelatedEntityId = id;
        dto.IsRead = true;
        dto.CreatedDate = date;

        dto.Id.Should().Be(id);
        dto.Title.Should().Be("Title");
        dto.Message.Should().Be("Message");
        dto.Type.Should().Be(NotificationType.System);
        dto.RelatedEntityId.Should().Be(id);
        dto.IsRead.Should().BeTrue();
        dto.CreatedDate.Should().Be(date);
    }
}
