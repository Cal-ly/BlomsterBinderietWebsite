namespace HttpWebshopCookie.Services;

/// <summary>
/// Service class for sending emails.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EmailService"/> class.
/// </remarks>
/// <param name="smtpSettings">The SMTP settings.</param>
public class EmailService(IOptions<SmtpSettings> smtpSettings) : IEmailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettings.Value;

    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="toEmail">The recipient email address.</param>
    /// <param name="subject">The email subject.</param>
    /// <param name="message">The email message.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the recipient email address, subject, or message is null or empty.</exception>
    /// <exception cref="SmtpCommandException">Thrown when the SMTP server returns an error response to a command.</exception>
    /// <exception cref="SmtpProtocolException">Thrown when an SMTP protocol error occurs.</exception>
    public async Task SendEmailAsync(string toEmail, string subject, string message)
    {
        if (string.IsNullOrEmpty(toEmail))
        {
            throw new ArgumentNullException(nameof(toEmail), "Recipient email address cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(subject))
        {
            throw new ArgumentNullException(nameof(subject), "Email subject cannot be null or empty.");
        }

        if (string.IsNullOrEmpty(message))
        {
            throw new ArgumentNullException(nameof(message), "Email message cannot be null or empty.");
        }

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(_smtpSettings.SenderName, _smtpSettings.SenderEmail));
        emailMessage.To.Add(new MailboxAddress("", toEmail));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = message };

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (SmtpCommandException ex)
        {
            // Handle command errors (e.g., invalid recipient address)
            throw new InvalidOperationException($"SMTP command error: {ex.Message}", ex);
        }
        catch (SmtpProtocolException ex)
        {
            // Handle protocol errors (e.g., unexpected server response)
            throw new InvalidOperationException($"SMTP protocol error: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Sends a MIME message asynchronously.
    /// </summary>
    /// <param name="message">The MIME message to send.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the MIME message is null.</exception>
    /// <exception cref="SmtpCommandException">Thrown when the SMTP server returns an error response to a command.</exception>
    /// <exception cref="SmtpProtocolException">Thrown when an SMTP protocol error occurs.</exception>
    public async Task SendMimeMessageAsync(MimeMessage message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message), "MIME message cannot be null.");
        }

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(_smtpSettings.Server, _smtpSettings.Port, MailKit.Security.SecureSocketOptions.StartTlsWhenAvailable);
            await client.AuthenticateAsync(_smtpSettings.Username, _smtpSettings.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (SmtpCommandException ex)
        {
            // Handle command errors (e.g., invalid recipient address)
            throw new InvalidOperationException($"SMTP command error: {ex.Message}", ex);
        }
        catch (SmtpProtocolException ex)
        {
            // Handle protocol errors (e.g., unexpected server response)
            throw new InvalidOperationException($"SMTP protocol error: {ex.Message}", ex);
        }
    }
}

/// <summary>
/// Configuration settings for SMTP.
/// </summary>
public class SmtpSettings
{
    /// <summary>
    /// Gets or sets the SMTP server.
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    /// Gets or sets the SMTP port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the sender's name.
    /// </summary>
    public string? SenderName { get; set; }

    /// <summary>
    /// Gets or sets the sender's email address.
    /// </summary>
    public string? SenderEmail { get; set; }

    /// <summary>
    /// Gets or sets the username for SMTP authentication.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Gets or sets the password for SMTP authentication.
    /// </summary>
    public string? Password { get; set; }
}
