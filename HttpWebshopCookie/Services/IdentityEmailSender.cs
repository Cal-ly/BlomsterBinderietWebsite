namespace HttpWebshopCookie.Services
{
    /// <summary>
    /// Represents a class that sends emails using the identity service.
    /// </summary>
    public class IdentityEmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityEmailSender"/> class.
        /// </summary>
        /// <param name="emailService">The email service to use for sending emails.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="emailService"/> is null.</exception>
        public IdentityEmailSender(IEmailService emailService)
        {
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService), "Email service cannot be null.");
        }

        /// <summary>
        /// Sends an email asynchronously.
        /// </summary>
        /// <param name="email">The recipient email address.</param>
        /// <param name="subject">The email subject.</param>
        /// <param name="htmlMessage">The HTML content of the email.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="email"/>, <paramref name="subject"/>, or <paramref name="htmlMessage"/> is null or empty.</exception>
        /// <exception cref="InvalidOperationException">Thrown when there is an error sending the email.</exception>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(nameof(email), "Recipient email address cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentNullException(nameof(subject), "Email subject cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(htmlMessage))
            {
                throw new ArgumentNullException(nameof(htmlMessage), "Email message cannot be null or empty.");
            }

            try
            {
                await _emailService.SendEmailAsync(email, subject, htmlMessage);
                return;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"An error occurred while sending the email: {ex.Message}", ex);
            }
        }
    }
}