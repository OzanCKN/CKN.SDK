#pragma warning disable CS0618 // Type or member is obsolete

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Notification.Push;

public class FirebaseNotificationOptions
{
    public const string SectionName = "Notification:Firebase";
    public string JsonCredentialPath { get; set; } = string.Empty;
}

public class FirebaseNotificationProvider : INotificationService
{
    public FirebaseNotificationProvider(IOptions<FirebaseNotificationOptions> options)
    {
        var opt = options.Value;
        if (FirebaseApp.DefaultInstance == null && !string.IsNullOrEmpty(opt.JsonCredentialPath))
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(opt.JsonCredentialPath)
            });
        }
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        => throw new System.NotSupportedException("Firebase Provider only supports push notifications.");

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        => throw new System.NotSupportedException("Firebase Provider only supports push notifications.");

    public async Task SendPushNotificationAsync(string deviceToken, string title, string body, object? data = null, CancellationToken cancellationToken = default)
    {
        var message = new Message
        {
            Token = deviceToken,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = body
            }
        };

        if (data is Dictionary<string, string> dataDict)
        {
            message.Data = dataDict;
        }

        await FirebaseMessaging.DefaultInstance.SendAsync(message, cancellationToken);
    }
}
