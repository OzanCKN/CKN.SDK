using System.Threading;
using System.Threading.Tasks;

namespace CKN.Sdk.Notification;

/// <summary>
/// Core notification service interface for sending various types of notifications.
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends an email notification.
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an SMS notification.
    /// </summary>
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a push notification (e.g. via Firebase Cloud Messaging).
    /// </summary>
    Task SendPushNotificationAsync(string deviceToken, string title, string body, object? data = null, CancellationToken cancellationToken = default);
}
