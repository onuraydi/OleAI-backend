using System.Collections.Generic;
using System.Threading.Tasks;

namespace OleAI.Backend.Application.Services.PushNotifications
{
    public interface IPushNotificationService
    {
        Task<bool> SendPushNotificationAsync(string deviceToken, string title, string body, Dictionary<string, string>? data = null);
        Task<bool> SendMulticastPushNotificationAsync(IEnumerable<string> deviceTokens, string title, string body, Dictionary<string, string>? data = null);
    }
}
