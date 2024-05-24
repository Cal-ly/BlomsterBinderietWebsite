using Microsoft.EntityFrameworkCore;

namespace HttpWebshopCookie.Services;

public class TagService(ApplicationDbContext context)
{
    public async Task<List<Tag>> GetTagsAsync()
    {
        return await context.Tags.ToListAsync();
    }

    public async Task<Tag> GetTagByIdAsync(string id)
    {
        var tag = await context.Tags.FindAsync(id);
        if (tag == null)
        {
            throw new KeyNotFoundException();
        }
        else
        {
            return tag;
        }
    }

    public async Task<Tag> CreateTagAsync(Tag tag)
    {
        context.Tags.Add(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> UpdateTagAsync(Tag tag)
    {
        context.Tags.Update(tag);
        await context.SaveChangesAsync();
        return tag;
    }

    public async Task<Tag> DeleteTagAsync(string id)
    {
        Tag? tag = await context.Tags.FindAsync(id);
        if (tag == null)
        {
            throw new KeyNotFoundException();
        }
        context.Tags.Remove(tag);
        await context.SaveChangesAsync();
        return tag;
    }
    public async Task<List<string?>> GetOccasionsAsync()
    {
        return await context.Tags
            .Where(t => t.Occasion != string.Empty)
            .Select(t => t.Occasion)
            .Distinct()
            .ToListAsync();
    }
    public async Task<List<Tag>> GetTagsOrderedByOccasionAsync()
    {
        return await context.Tags
                             .OrderBy(t => t.Occasion)
                             .ThenBy(t => t.Category)
                             .ThenBy(t => t.SubCategory)
                             .ToListAsync();
    }
    public async Task<(List<Tag>, int)> GetTagsOrderedByOccasionAsync(int pageNumber, int pageSize)
    {
        var query = context.Tags
                             .OrderBy(t => t.Occasion)
                             .ThenBy(t => t.Category)
                             .ThenBy(t => t.SubCategory);

        var totalItems = await query.CountAsync();
        var tags = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return (tags, totalItems);
    }
}