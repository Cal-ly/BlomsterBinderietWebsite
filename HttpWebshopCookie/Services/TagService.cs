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
}