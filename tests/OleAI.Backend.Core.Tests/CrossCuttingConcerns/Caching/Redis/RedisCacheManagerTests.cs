using System.Text.Json;
using Moq;
using OleAI.Backend.Core.CrossCuttingConcerns.Caching.Redis;
using StackExchange.Redis;
using Xunit;

namespace OleAI.Backend.Core.Tests.CrossCuttingConcerns.Caching.Redis;

public class RedisCacheManagerTests
{
    private readonly Mock<IConnectionMultiplexer> _redisConnectionMock;
    private readonly Mock<IDatabase> _databaseMock;
    private readonly RedisCacheManager _cacheManager;

    public RedisCacheManagerTests()
    {
        _redisConnectionMock = new Mock<IConnectionMultiplexer>();
        _databaseMock = new Mock<IDatabase>();
        _redisConnectionMock.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_databaseMock.Object);
        _cacheManager = new RedisCacheManager(_redisConnectionMock.Object);
    }

    [Fact]
    public async Task ExistsAsync_ShouldCallKeyExists()
    {
        // Arrange
        var key = (RedisKey)"test-key";
        _databaseMock.Setup(x => x.KeyExistsAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(true);

        // Act
        var result = await _cacheManager.ExistsAsync("test-key");

        // Assert
        Assert.True(result);
        _databaseMock.Verify(x => x.KeyExistsAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenValueExists_ShouldDeserialize()
    {
        // Arrange
        var testObj = new { Name = "Test" };
        var json = JsonSerializer.Serialize(testObj);
        var key = (RedisKey)"test-key";
        _databaseMock.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync((RedisValue)json);

        // Act
        var result = await _cacheManager.GetAsync<dynamic>("test-key");

        // Assert
        Assert.NotNull(result);
        _databaseMock.Verify(x => x.StringGetAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WhenValueDoesNotExist_ShouldReturnDefault()
    {
        // Arrange
        var key = (RedisKey)"test-key";
        _databaseMock.Setup(x => x.StringGetAsync(key, It.IsAny<CommandFlags>())).ReturnsAsync(RedisValue.Null);

        // Act
        var result = await _cacheManager.GetAsync<string>("test-key");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveAsync_ShouldCallKeyDelete()
    {
        // Act
        await _cacheManager.RemoveAsync("test-key");

        // Assert
        var key = (RedisKey)"test-key";
        _databaseMock.Verify(x => x.KeyDeleteAsync(key, It.IsAny<CommandFlags>()), Times.Once);
    }

    [Fact]
    public async Task SetAsync_ShouldCallStringSet()
    {
        // Arrange
        var testValue = "test-value";
        var expiry = TimeSpan.FromMinutes(10);
        var key = (RedisKey)"test-key";
        var val = (RedisValue)JsonSerializer.Serialize(testValue);

        // Act
        await _cacheManager.SetAsync("test-key", testValue, absoluteExpiration: expiry);

        // Assert
        // SENIOR TRICK: To avoid CS0854 (optional arguments in expression tree), 
        // we use It.IsAny for the optional parameters that we don't strictly need to match EXACTLY or
        // we ensure all are matched if the compiler allows.
        // Actually, using It.IsAny for ALL trailing optional params often works better.
        _databaseMock.Verify(x => x.StringSetAsync(
            key,
            val,
            expiry,
            false,
            When.Always,
            It.IsAny<CommandFlags>()), Times.Once);
    }
}
