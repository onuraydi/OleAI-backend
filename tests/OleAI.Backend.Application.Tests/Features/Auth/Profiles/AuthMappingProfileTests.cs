using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using OleAI.Backend.Application.Features.Auth.Dtos.Requests;
using OleAI.Backend.Application.Features.Auth.Dtos.Responses;
using OleAI.Backend.Application.Features.Auth.Profiles;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Auth.Profiles;

public class AuthMappingProfileTests
{
    private readonly IMapper _mapper;

    public AuthMappingProfileTests()
    {
        var services = new ServiceCollection();
        services.AddLogging(); // Required by AutoMapper DI
        services.AddAutoMapper(cfg => cfg.AddProfile<AuthMappingProfile>());
        var provider = services.BuildServiceProvider();
        _mapper = provider.GetRequiredService<IMapper>();
    }

    [Fact]
    public void Construct_ShouldHaveValidConfiguration()
    {
        var request = new RegisterRequest { Email = "t@t.com", Password = "p", FirstName = "F", LastName = "L" };
        var user = new User { Id = Guid.NewGuid(), Email = "t@t.com", FirstName = "F", LastName = "L" };

        var act1 = () => _mapper.Map<User>(request);
        var act2 = () => _mapper.Map<RegisteredResponse>(user);
        var act3 = () => _mapper.Map<verifiedEmailResponse>(user);

        act1.Should().NotThrow();
        act2.Should().NotThrow();
        act3.Should().NotThrow();
    }

    [Fact]
    public void Map_RegisterRequest_To_User_ShouldMapCorrectly()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "test@test.com",
            Password = "password",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var user = _mapper.Map<User>(request);

        // Assert
        user.Email.Should().Be(request.Email);
        user.FirstName.Should().Be(request.FirstName);
        user.LastName.Should().Be(request.LastName);
    }

    [Fact]
    public void Map_User_To_RegisteredResponse_ShouldMapCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com",
            FirstName = "Test",
            LastName = "User"
        };

        // Act
        var response = _mapper.Map<RegisteredResponse>(user);

        // Assert
        response.Id.Should().Be(user.Id);
        response.Email.Should().Be(user.Email);
    }

    [Fact]
    public void Map_User_To_verifiedEmailResponse_ShouldMapCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@test.com"
        };

        // Act
        var response = _mapper.Map<verifiedEmailResponse>(user);

        // Assert
        response.Email.Should().Be(user.Email);
    }
}
