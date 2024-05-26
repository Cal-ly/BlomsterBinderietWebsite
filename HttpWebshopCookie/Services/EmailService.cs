namespace HttpWebshopCookie.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;

    public EmailService(IOptions<SmtpSettings> smtpSettings)
    {
        _smtpSettings = smtpSettings.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        emailMessage.To.Add(new MailboxAddress("", toEmail));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.None);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }

    public async Task SendMimeMessageAsync(MimeMessage message)
    {
        using (var client = new SmtpClient())
        {
            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.None);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}

public class SmtpSettings
{
    public string? Server { get; set; }
    public int Port { get; set; }
    public string? SenderName { get; set; }
    public string? SenderEmail { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}