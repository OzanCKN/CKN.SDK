using System;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

using CKN.Sdk.Notification;

public static class NotificationConfigurationExtensions
{
    /// <summary>
    /// Base registration for the Notification module.
    /// Specific implementations (SMTP, SendGrid, Twilio, Firebase) must be registered separately.
    /// </summary>
    public static IServiceCollection AddCknNotifications(this IServiceCollection services)
    {
        // Core notification logic or dispatchers can be registered here.
        return services;
    }

    public static IServiceCollection AddCknSmtpNotification(this IServiceCollection services, Action<CKN.Sdk.Notification.Email.SmtpNotificationOptions> configure)
    {
        services.Configure(configure);
        services.AddScoped<INotificationService, CKN.Sdk.Notification.Email.SmtpNotificationProvider>();
        return services;
    }

    public static IServiceCollection AddCknSendGridNotification(this IServiceCollection services, Action<CKN.Sdk.Notification.Email.SendGridNotificationOptions> configure)
    {
        services.Configure(configure);
        services.AddScoped<INotificationService, CKN.Sdk.Notification.Email.SendGridNotificationProvider>();
        return services;
    }

    public static IServiceCollection AddCknTwilioNotification(this IServiceCollection services, Action<CKN.Sdk.Notification.Sms.TwilioNotificationOptions> configure)
    {
        services.Configure(configure);
        services.AddScoped<INotificationService, CKN.Sdk.Notification.Sms.TwilioNotificationProvider>();
        return services;
    }

    public static IServiceCollection AddCknFirebaseNotification(this IServiceCollection services, Action<CKN.Sdk.Notification.Push.FirebaseNotificationOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<INotificationService, CKN.Sdk.Notification.Push.FirebaseNotificationProvider>();
        return services;
    }
}
