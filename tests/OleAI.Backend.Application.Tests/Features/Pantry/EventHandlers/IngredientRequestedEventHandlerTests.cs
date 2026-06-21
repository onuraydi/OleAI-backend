using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OleAI.Backend.Application.Features.Pantry.EventHandlers;
using OleAI.Backend.Application.Features.Pantry.Events;
using OleAI.Backend.Application.Services.Llm;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Pantry.EventHandlers;

public class IngredientRequestedEventHandlerTests
{
    private readonly Mock<IIngredientRequestRepository> _mockIngredientRequestRepository;
    private readonly Mock<ILocalLlmService> _mockLocalLlmService;
    private readonly IngredientRequestedEventHandler _handler;

    public IngredientRequestedEventHandlerTests()
    {
        _mockIngredientRequestRepository = new Mock<IIngredientRequestRepository>();
        _mockLocalLlmService = new Mock<ILocalLlmService>();

        var mockServiceProvider = new Mock<IServiceProvider>();
        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(IIngredientRequestRepository)))
            .Returns(_mockIngredientRequestRepository.Object);
        mockServiceProvider
            .Setup(sp => sp.GetService(typeof(ILocalLlmService)))
            .Returns(_mockLocalLlmService.Object);

        var mockServiceScope = new Mock<IServiceScope>();
        mockServiceScope.Setup(s => s.ServiceProvider).Returns(mockServiceProvider.Object);

        var mockScopeFactory = new Mock<IServiceScopeFactory>();
        mockScopeFactory.Setup(f => f.CreateScope()).Returns(mockServiceScope.Object);

        _handler = new IngredientRequestedEventHandler(mockScopeFactory.Object);
    }

    [Fact]
    public async Task Handle_WhenRequestNotFound_ShouldDoNothing()
    {
        // Arrange
        var notification = new IngredientRequestedEvent { RequestId = Guid.NewGuid(), RequestedName = "New Food" };

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((IngredientRequest?)null);

        // Act
        await _handler.Handle(notification, CancellationToken.None);
        await Task.Delay(100); // Wait for fire-and-forget background task

        // Assert
        _mockLocalLlmService.Verify(l => l.AnalyzeIngredientRequestAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        _mockIngredientRequestRepository.Verify(r => r.UpdateAsync(It.IsAny<IngredientRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenLlmReturnsResult_ShouldUpdateRequestAnalysis()
    {
        // Arrange
        var request = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Domat");
        var notification = new IngredientRequestedEvent { RequestId = request.Id, RequestedName = request.RequestedName };
        const string llmJson = "{\"IsNew\":false}";

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _mockLocalLlmService
            .Setup(l => l.AnalyzeIngredientRequestAsync(request.RequestedName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(llmJson);

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        // Act
        await _handler.Handle(notification, CancellationToken.None);
        await Task.Delay(100); // Wait for fire-and-forget background task

        // Assert
        request.LlmAnalysisResult.Should().Be(llmJson);
        _mockIngredientRequestRepository.Verify(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenLlmThrows_ShouldPersistErrorJson()
    {
        // Arrange
        var request = new IngredientRequest(Guid.NewGuid(), Guid.NewGuid(), "Domat");
        var notification = new IngredientRequestedEvent { RequestId = request.Id, RequestedName = request.RequestedName };

        _mockIngredientRequestRepository
            .Setup(r => r.GetAsync(It.IsAny<Expression<Func<IngredientRequest, bool>>>(), null, false, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _mockLocalLlmService
            .Setup(l => l.AnalyzeIngredientRequestAsync(request.RequestedName, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("LLM kapalı"));

        _mockIngredientRequestRepository
            .Setup(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        // Act
        await _handler.Handle(notification, CancellationToken.None);
        await Task.Delay(100); // Wait for fire-and-forget background task

        // Assert
        request.LlmAnalysisResult.Should().NotBeNull();
        request.LlmAnalysisResult.Should().Contain("LLM Analizi başarısız oldu");
        request.LlmAnalysisResult.Should().Contain("LLM kapalı");

        _mockIngredientRequestRepository.Verify(r => r.UpdateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
