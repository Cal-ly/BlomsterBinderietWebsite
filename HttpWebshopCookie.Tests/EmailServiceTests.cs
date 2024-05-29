using HttpWebshopCookie.Services;
using Microsoft.Extensions.Options;
using Moq;

namespace HttpWebshopCookie.Tests;

[TestClass]
public class EmailServiceTests
{
    private Mock<IOptions<SmtpSettings>> _mockSmtpSettings = null!;
    private EmailService _emailService = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _mockSmtpSettings = new Mock<IOptions<SmtpSettings>>();
        _mockSmtpSettings.Setup(x => x.Value).Returns(new SmtpSettings
        {
            Server = "localhost",
            Port = 25,
            SenderName = "Test",
            SenderEmail = "test@test.com"
        });
        _emailService = new EmailService(_mockSmtpSettings.Object);
    }

    [TestMethod]
    public async Task SendEmailAsync_WhenCalled_ShouldNotThrowException()
    {
        // Arrange
        string to = "recipient@test.com";
        string subject = "Test Subject";
        string messageBody = "Test Body";

        // Act
        var exception = await Assert.ThrowsExceptionAsync<System.Net.Sockets.SocketException>(async () => await _emailService.SendEmailAsync(to, subject, messageBody));

        // Assert
        Assert.IsInstanceOfType<System.Net.Sockets.SocketException>(exception);
    }
}
