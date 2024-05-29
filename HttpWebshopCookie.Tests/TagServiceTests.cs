using HttpWebshopCookie.Data;
using HttpWebshopCookie.Models;
using HttpWebshopCookie.Services;
using Microsoft.EntityFrameworkCore;

namespace HttpWebshopCookie.Tests;

[TestClass]
public class TagServiceTests
{
    private TagService? _tagService;
    private ApplicationDbContext? _context;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _context = new ApplicationDbContext(options);
        _tagService = new TagService(_context);
    }

    [TestMethod]
    public async Task CreateTagAsync_ShouldAddTagToDatabase()
    {
        // Arrange
        var tag = new Tag { Occasion = "Test Occasion", Category = "Test Category", SubCategory = "Test SubCategory" };

        // Act
        await _tagService!.CreateTagAsync(tag);
        var tags = await _context!.Tags.ToListAsync();

        // Assert
        Assert.AreEqual(1, tags.Count);
        Assert.AreEqual("Test Occasion", tags[0].Occasion);
    }
}
