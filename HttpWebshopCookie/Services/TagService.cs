namespace HttpWebshopCookie.Services;

/// <summary>
/// Service class for managing tags.
/// </summary>
public class TagService
{
    private readonly ApplicationDbContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagService"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <exception cref="ArgumentNullException">Thrown when the context is null.</exception>
    public TagService(ApplicationDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context), "Database context cannot be null.");
    }

    /// <summary>
    /// Retrieves all tags asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of tags.</returns>
    /// <exception cref="InvalidOperationException">Thrown when unable to retrieve tags.</exception>
    public async Task<List<Tag>> GetTagsAsync()
    {
        try
        {
            return await context.Tags.ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to retrieve tags.", ex);
        }
    }

    /// <summary>
    /// Retrieves a tag by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the tag.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the tag with the specified ID.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the ID is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the tag with the specified ID is not found.</exception>
    public async Task<Tag> GetTagByIdAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id), "Tag ID cannot be null or empty.");
        }

        var tag = await context.Tags.FindAsync(id);
        return tag ?? throw new KeyNotFoundException("Tag not found.");
    }

    /// <summary>
    /// Creates a new tag asynchronously.
    /// </summary>
    /// <param name="tag">The tag to create.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the created tag.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the tag is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unable to create the tag.</exception>
    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag), "Tag cannot be null.");
        }

        try
        {
            context.Tags.Add(tag);
            await context.SaveChangesAsync();
            return tag;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to create tag.", ex);
        }
    }

    /// <summary>
    /// Updates an existing tag asynchronously.
    /// </summary>
    /// <param name="tag">The tag to update.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the updated tag.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the tag is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unable to update the tag.</exception>
    public async Task<Tag> UpdateTagAsync(Tag tag)
    {
        if (tag == null)
        {
            throw new ArgumentNullException(nameof(tag), "Tag cannot be null.");
        }

        try
        {
            context.Tags.Update(tag);
            await context.SaveChangesAsync();
            return tag;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to update tag.", ex);
        }
    }

    /// <summary>
    /// Deletes a tag by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the tag to delete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the deleted tag.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the ID is null or empty.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the tag with the specified ID is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when unable to delete the tag.</exception>
    public async Task<Tag> DeleteTagAsync(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id), "Tag ID cannot be null or empty.");
        }

        var tag = await context.Tags.FindAsync(id);
        if (tag == null)
        {
            throw new KeyNotFoundException("Tag not found.");
        }

        try
        {
            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
            return tag;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to delete tag.", ex);
        }
    }

    /// <summary>
    /// Retrieves all unique occasions asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of unique occasions.</returns>
    /// <exception cref="InvalidOperationException">Thrown when unable to retrieve occasions.</exception>
    public async Task<List<string?>> GetOccasionsAsync()
    {
        try
        {
            return await context.Tags
                .Where(t => t.Occasion != string.Empty)
                .Select(t => t.Occasion)
                .Distinct()
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to retrieve occasions.", ex);
        }
    }

    /// <summary>
    /// Retrieves all tags ordered by occasion, category, and subcategory asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of tags ordered by occasion, category, and subcategory.</returns>
    /// <exception cref="InvalidOperationException">Thrown when unable to retrieve ordered tags.</exception>
    public async Task<List<Tag>> GetTagsOrderedByOccasionAsync()
    {
        try
        {
            return await context.Tags
                                 .OrderBy(t => t.Occasion)
                                 .ThenBy(t => t.Category)
                                 .ThenBy(t => t.SubCategory)
                                 .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to retrieve ordered tags.", ex);
        }
    }

    /// <summary>
    /// Retrieves a paged list of tags ordered by occasion, category, and subcategory asynchronously.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a tuple with the paged list of tags and the total number of items.</returns>
    /// <exception cref="InvalidOperationException">Thrown when unable to retrieve paged tags.</exception>
    public async Task<(List<Tag>, int)> GetTagsOrderedByOccasionAsync(int pageNumber, int pageSize)
    {
        try
        {
            var query = context.Tags
                                 .OrderBy(t => t.Occasion)
                                 .ThenBy(t => t.Category)
                                 .ThenBy(t => t.SubCategory);

            var totalItems = await query.CountAsync();
            var tags = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return (tags, totalItems);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to retrieve paged tags.", ex);
        }
    }
}
