using Microsoft.AspNetCore.Mvc.Testing;

namespace HttpWebshopCookie.Tests;

[TestClass]
public class IndexPageTests
{
    private readonly WebApplicationFactory<TestStartup.TestStartup> _factory;

    public IndexPageTests()
    {
        _factory = new WebApplicationFactory<TestStartup.TestStartup>();
    }

    [TestMethod]
    public async Task Get_IndexPage_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.AreEqual("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());
    }
}
