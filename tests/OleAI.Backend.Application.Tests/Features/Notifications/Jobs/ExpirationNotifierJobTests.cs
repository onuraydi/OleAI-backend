using FluentAssertions;
using Moq;
using OleAI.Backend.Application.Features.Notifications.Jobs;
using OleAI.Backend.Application.Services.PushNotifications;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Core.Repositories.Options;
using OleAI.Backend.Core.Repositories.Pagination;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;
using Xunit;

namespace OleAI.Backend.Application.Tests.Features.Notifications.Jobs;

public class ExpirationNotifierJobTests
{
    private readonly Mock<IUserIngredientRepository> _userIngredientRepositoryMock;
    private readonly Mock<INotificationRepository> _notificationRepositoryMock;
    private readonly Mock<IUserDeviceRepository> _userDeviceRepositoryMock;
    private readonly Mock<IPushNotificationService> _pushNotificationServiceMock;
    private readonly ExpirationNotifierJob _job;

    public ExpirationNotifierJobTests()
    {
        _userIngredientRepositoryMock = new Mock<IUserIngredientRepository>();
        _notificationRepositoryMock = new Mock<INotificationRepository>();
        _userDeviceRepositoryMock = new Mock<IUserDeviceRepository>();
        _pushNotificationServiceMock = new Mock<IPushNotificationService>();

        _job = new ExpirationNotifierJob(
            _userIngredientRepositoryMock.Object,
            _notificationRepositoryMock.Object,
            _userDeviceRepositoryMock.Object,
            _pushNotificationServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoExpiringIngredients_ShouldNotCreateNotifications()
    {
        // Arrange
        var paginatedIngredients = new Paginate<UserIngredient> { Items = new List<UserIngredient>() };

        _userIngredientRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedIngredients);

        // Act
        await _job.ExecuteAsync();

        // Assert
        _notificationRepositoryMock.Verify(x => x.AddRangeAsync(It.IsAny<IEnumerable<Notification>>(), It.IsAny<CancellationToken>()), Times.Never);
        _pushNotificationServiceMock.Verify(x => x.SendMulticastPushNotificationAsync(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExpiringIngredientsExist_ShouldCreateNotificationsAndSendPush()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 500, MeasurementUnit.Gram, DateTime.UtcNow.AddDays(3));

        var paginatedIngredients = new Paginate<UserIngredient> { Items = new List<UserIngredient> { userIngredient } };

        var userDevice = new UserDevice(Guid.NewGuid(), userId, "token-123", DevicePlatform.Android);
        var paginatedDevices = new Paginate<UserDevice> { Items = new List<UserDevice> { userDevice } };

        _userIngredientRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedIngredients);

        _userDeviceRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<UserDevice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedDevices);

        // Act
        await _job.ExecuteAsync();

        // Assert
        _notificationRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<Notification>>(n => n.Count() == 1), It.IsAny<CancellationToken>()), Times.Once);
        _pushNotificationServiceMock.Verify(x => x.SendMulticastPushNotificationAsync(
            It.Is<List<string>>(tokens => tokens.Contains("token-123")),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExpiringIngredientsExistButNoDevices_ShouldCreateNotificationsButNotSendPush()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var ingredientId = Guid.NewGuid();
        var userIngredient = UserIngredient.CreateStandard(Guid.NewGuid(), userId, ingredientId, 500, MeasurementUnit.Gram, DateTime.UtcNow.AddDays(3));

        var paginatedIngredients = new Paginate<UserIngredient> { Items = new List<UserIngredient> { userIngredient } };

        var paginatedDevices = new Paginate<UserDevice> { Items = new List<UserDevice>() }; // No devices

        _userIngredientRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<UserIngredient>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedIngredients);

        _userDeviceRepositoryMock.Setup(x => x.GetListAsync(It.IsAny<QueryOptions<UserDevice>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paginatedDevices);

        // Act
        await _job.ExecuteAsync();

        // Assert
        _notificationRepositoryMock.Verify(x => x.AddRangeAsync(It.Is<IEnumerable<Notification>>(n => n.Count() == 1), It.IsAny<CancellationToken>()), Times.Once);
        _pushNotificationServiceMock.Verify(x => x.SendMulticastPushNotificationAsync(It.IsAny<List<string>>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>()), Times.Never);
    }
}
