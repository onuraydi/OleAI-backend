using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace OleAI.Backend.Application.Services.PushNotifications
{
    public class FirebasePushNotificationManager : IPushNotificationService
    {
        private readonly ILogger<FirebasePushNotificationManager> _logger;

        public FirebasePushNotificationManager(ILogger<FirebasePushNotificationManager> logger)
        {
            _logger = logger;
        }

        public async Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null)
        {
            try
            {
                var message = new Message()
                {
                    Token = deviceToken,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                    },
                    Data = data
                };

                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                _logger.LogInformation("Successfully sent message: {response}", response);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending push notification to token {token}", deviceToken);
                return false;
            }
        }

        public async Task<bool> SendMulticastPushNotificationAsync(IEnumerable<string> deviceTokens, string title, string body, Dictionary<string, string>? data = null)
        {
            try
            {
                var message = new MulticastMessage()
                {
                    Tokens = (IReadOnlyList<string>)deviceTokens,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                    },
                    Data = data
                };

                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
                _logger.LogInformation("{successCount} messages were sent successfully", response.SuccessCount);
                return response.SuccessCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending multicast push notifications.");
                return false;
            }
        }
    }
}
