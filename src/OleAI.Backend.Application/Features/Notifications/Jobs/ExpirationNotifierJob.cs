using OleAI.Backend.Application.Services.PushNotifications;
using OleAI.Backend.Application.Services.Repositories;
using OleAI.Backend.Domain.Entities;
using OleAI.Backend.Domain.Enums;

namespace OleAI.Backend.Application.Features.Notifications.Jobs
{
    public class ExpirationNotifierJob
    {
        private readonly IUserIngredientRepository _userIngredientRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserDeviceRepository _userDeviceRepository;
        private readonly IPushNotificationService _pushNotificationService;

        public ExpirationNotifierJob(IUserIngredientRepository userIngredientRepository, INotificationRepository notificationRepository, IUserDeviceRepository userDeviceRepository, IPushNotificationService pushNotificationService)
        {
            _userIngredientRepository = userIngredientRepository;
            _notificationRepository = notificationRepository;
            _userDeviceRepository = userDeviceRepository;
            _pushNotificationService = pushNotificationService;
        }

        public async Task ExecuteAsync()
        {
            // SKT'si 3 gün kalan (bugün hariç 3 gün) ürünleri bul.
            // TimeSpan hesaplamak yerine basitçe Date karşılaştırması yapabiliriz.
            var targetDate = DateTime.UtcNow.Date.AddDays(3);

            var expiringIngredients = await _userIngredientRepository.GetListAsync(
                new OleAI.Backend.Core.Repositories.Options.QueryOptions<UserIngredient>
                {
                    Predicate = ui => ui.ExpirationDate != null
                                 && ui.ExpirationDate.Value.Date == targetDate.Date
                                 && ui.Amount > 0,
                    Size = 10000
                });

            var notificationsToCreate = new List<Notification>();

            foreach (var item in expiringIngredients.Items)
            {
                // Hali hazırda bu ürün için okumadığı bir SKT uyarısı varsa tekrarlama? 
                // Şimdilik basitçe: "Bu malzeme 3 gün içinde bitiyor!"

                string ingredientName = item.IngredientAlias?.AliasName ?? item.Ingredient?.Name ?? "Bilinmeyen Malzeme";

                var notification = new Notification(
                    Guid.NewGuid(),
                    item.UserId,
                    "Son Kullanma Tarihi Yaklaşıyor!",
                    $"Dolabınızdaki {ingredientName} adlı malzemenin son kullanma tarihi yaklaşmaktadır. Ziyan olmaması için değerlendirmeyi unutmayın.",
                    NotificationType.Expiration,
                    item.Id // RelatedEntityId = UserIngredientId
                );

                notificationsToCreate.Add(notification);
            }

            if (notificationsToCreate.Count > 0)
            {
                await _notificationRepository.AddRangeAsync(notificationsToCreate);

                // Group notifications by user to send push notifications
                var userGroups = notificationsToCreate.GroupBy(n => n.UserId);
                foreach (var group in userGroups)
                {
                    var userId = group.Key;
                    var devices = await _userDeviceRepository.GetListAsync(
                        new OleAI.Backend.Core.Repositories.Options.QueryOptions<UserDevice>
                        {
                            Predicate = d => d.UserId == userId
                        });

                    var deviceTokens = devices.Items.Select(d => d.DeviceToken).ToList();

                    if (deviceTokens.Any())
                    {
                        foreach (var notification in group)
                        {
                            await _pushNotificationService.SendMulticastPushNotificationAsync(
                                deviceTokens,
                                notification.Title,
                                notification.Message
                            );
                        }
                    }
                }
            }
        }
    }
}
