using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Options;

namespace CKN.Sdk.Notification.Email;

public class SmtpNotificationOptions
{
    public const string SectionName = "Notification:Smtp";
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; } = 587;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FromAddress { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
}

public class SmtpNotificationProvider : INotificationService
{
    private readonly SmtpNotificationOptions _options;

    public SmtpNotificationProvider(IOptions<SmtpNotificationOptions> options)
    {
        _options = options.Value;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_options.FromName, _options.FromAddress));
        message.To.Add(new MailboxAddress("", to));
        message.Subject = subject;
        
        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_options.Host, _options.Port, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(_options.Username, _options.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        => throw new System.NotSupportedException("SMTP Provider only supports email.");

    public Task SendPushNotificationAsync(string deviceToken, string title, string body, object? data = null, CancellationToken cancellationToken = default)
        => throw new System.NotSupportedException("SMTP Provider only supports email.");
}
