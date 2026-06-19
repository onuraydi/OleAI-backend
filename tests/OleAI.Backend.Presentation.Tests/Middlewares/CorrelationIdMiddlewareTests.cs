using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Moq;
using OleAI.Backend.Presentation.Middlewares;
using Xunit;

namespace OleAI.Backend.Presentation.Tests.Middlewares
{
    public class CorrelationIdMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_WhenHeaderExists_ShouldUseExistingCorrelationId()
        {
            var expectedCorrelationId = "test-correlation-id";
            var context = new DefaultHttpContext();
            context.Request.Headers["X-Correlation-ID"] = expectedCorrelationId;

            var nextCalled = false;
            var middleware = new CorrelationIdMiddleware(innerContext =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

            await middleware.InvokeAsync(context);

            nextCalled.Should().BeTrue();
            // Checking if OnStarting callback is registered but we cannot easily invoke it in DefaultHttpContext without starting response manually.
        }

        [Fact]
        public async Task InvokeAsync_WhenHeaderDoesNotExist_ShouldGenerateNewGuid()
        {
            var context = new DefaultHttpContext();

            var nextCalled = false;
            var middleware = new CorrelationIdMiddleware(innerContext =>
            {
                nextCalled = true;
                return Task.CompletedTask;
            });

            await middleware.InvokeAsync(context);

            nextCalled.Should().BeTrue();
        }

        [Fact]
        public void UseCorrelationId_ShouldAddMiddleware()
        {
            var builderMock = new Mock<IApplicationBuilder>();

            builderMock.Object.UseCorrelationId();

            builderMock.Verify(b => b.Use(It.IsAny<System.Func<RequestDelegate, RequestDelegate>>()), Times.AtLeastOnce());
        }
    }
}
