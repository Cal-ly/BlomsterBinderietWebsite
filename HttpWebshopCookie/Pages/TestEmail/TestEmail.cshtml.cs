namespace HttpWebshopCookie.Pages.TestEmail
{
    [Authorize(Policy = "managerAccess")]
    public class TestEmailModel : PageModel
    {
        private readonly IEmailService _emailService;

        public TestEmailModel(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [BindProperty]
        public string? RecipientEmail { get; set; }

        [BindProperty]
        public string? EmailSubject { get; set; }

        [BindProperty]
        public string? EmailBody { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostSendTestEmailAsync()
        {
            if (string.IsNullOrEmpty(RecipientEmail) || string.IsNullOrEmpty(EmailSubject) || string.IsNullOrEmpty(EmailBody))
            {
                TempData["Message"] = "Please fill in all fields!";
                return Page();
            }
            await _emailService.SendEmailAsync(RecipientEmail, EmailSubject, EmailBody);
            TempData["Message"] = "Test email sent successfully!";
            return Page();
        }
    }
}
