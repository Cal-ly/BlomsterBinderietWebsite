namespace HttpWebshopCookie.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string message);

        Task SendMimeMessageAsync(MimeMessage message);
    }
}
