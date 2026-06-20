using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class ContactMessageTests
{
    [Fact]
    public void Constructor_ShouldInitializeCorrectly()
    {
        var id = Guid.NewGuid();
        var message = new ContactMessage(id, "Name", "test@test.com", "Department", "Message", false);

        message.Id.Should().Be(id);
        message.Name.Should().Be("Name");
        message.Email.Should().Be("test@test.com");
        message.Department.Should().Be("Department");
        message.Message.Should().Be("Message");
        message.IsRead.Should().BeFalse();
    }
}
