using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace CKN.Sdk.Notification.Email;

public class SendGridNotificationOptions
{
    public const string SectionName = "Notification:SendGrid";
    public string ApiKey { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

public class SendGridNotificationProvider : INotificationService
{
    private readonly SendGridNotificationOptions _options;
    private readonly ISendGridClient _client;

    public SendGridNotificationProvider(IOptions<SendGridNotificationOptions> options)
    {
        _options = options.Value;
        _client = new SendGridClient(_options.ApiKey);
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var from = new EmailAddress(_options.FromAddress, _options.FromName);
        var toAddress = new EmailAddress(to);
        var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);
        
        await _client.SendEmailAsync(msg, cancellationToken);
    }

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        => throw new System.NotSupportedException("SendGrid Provider only supports email.");

    public Task SendPushNotificationAsync(string deviceToken, string title, string body, object? data = null, CancellationToken cancellationToken = default)
        => throw new System.NotSupportedException("SendGrid Provider only supports email.");
}
