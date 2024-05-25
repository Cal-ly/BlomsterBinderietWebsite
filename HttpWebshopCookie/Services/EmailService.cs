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
        emailMessage.From.Add(MailboxAddress.Parse("test@test.com"));
        emailMessage.To.Add(MailboxAddress.Parse(toEmail));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message };

        using (var client = new SmtpClient())
        {
            await client.ConnectAsync("localhost", 25, false);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
    }
}

public class SmtpSettings
{
    public string Server { get; set; } = "localhost";
    public int Port { get; set; } = 25;
    public string SenderName { get; set; } = "localhost";
    public string SenderEmail { get; set; } = "test@test.com"; // This is a placeholder email address
    public string Username { get; set; } = ""; // This is a placeholder username
    public string Password { get; set; } = ""; // This is a placeholder password
}
