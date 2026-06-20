using System;
using System.Reflection;
using FluentAssertions;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Domain.Tests.Entities;

public class SystemLogTests
{
    [Fact]
    public void Constructor_ShouldInitialize()
    {
        var log = (SystemLog)Activator.CreateInstance(typeof(SystemLog), nonPublic: true)!;

        log.Should().NotBeNull();
        log.Message.Should().BeNull();
        log.Level.Should().BeNull();
    }
}
