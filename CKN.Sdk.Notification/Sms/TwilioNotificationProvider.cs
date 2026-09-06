using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace CKN.Sdk.Notification.Sms;

public class TwilioNotificationOptions
{
    public const string SectionName = "Notification:Twilio";
    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string FromPhoneNumber { get; set; } = string.Empty;
}

public class TwilioNotificationProvider : INotificationService
{
    private readonly TwilioNotificationOptions _options;

    public TwilioNotificationProvider(IOptions<TwilioNotificationOptions> options)
    {
        _options = options.Value;
        TwilioClient.Init(_options.AccountSid, _options.AuthToken);
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
        => throw new System.NotSupportedException("Twilio Provider only supports SMS.");

    public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        await MessageResource.CreateAsync(
            body: message,
            from: new Twilio.Types.PhoneNumber(_options.FromPhoneNumber),
            to: new Twilio.Types.PhoneNumber(phoneNumber)
        );
    }

    public Task SendPushNotificationAsync(string deviceToken, string title, string body, object? data = null, CancellationToken cancellationToken = default)
        => throw new System.NotSupportedException("Twilio Provider only supports SMS.");
}
