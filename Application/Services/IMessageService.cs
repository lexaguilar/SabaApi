using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;


public interface IMessageService
{
    Task<bool> SendAsync(MimeMessage message);
}

public class MessageService : IMessageService, IDisposable
{
    private readonly SmtpClient _client;
    private readonly ILogger _logger;
    private bool disposedValue;
    private string _sender;

    public MessageService(IOptions<SmtpOptions> options, ILogger<MessageService> logger)
    {
        var smtp = options.Value;
        _sender = smtp.DefaultSender;
        _client = new SmtpClient();
        _client.Connect(smtp.Host, smtp.Port, SecureSocketOptions.StartTls);
        _client.Authenticate(smtp.UserName, smtp.Password);
        _logger = logger;
    }

    public async Task<bool> SendAsync(MimeMessage message)
    {
        try
        {
            message.From.Add(MailboxAddress.Parse(_sender));
            _client.CheckCertificateRevocation = false;
            await _client.SendAsync(message);
            await _client.DisconnectAsync(true);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email");
            throw;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _client.Dispose();
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class SmtpOptions
{
    public string Host { get; set; } = String.Empty;
    public int Port { get; set; }
    public string DefaultSender { get; set; } = String.Empty;
    public string UserName { get; set; } = String.Empty;
    public string Password { get; set; } = String.Empty;
    public bool EnableSsl { get; set; } = false;
}
