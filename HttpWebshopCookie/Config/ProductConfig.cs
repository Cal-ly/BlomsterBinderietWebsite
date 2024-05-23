namespace HttpWebshopCookie.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.UpdatedAt).HasColumnType("datetime").HasDefaultValue(DateTime.UtcNow);

        builder.HasMany(p => p.ProductTags)
            .WithOne(pt => pt.Product)
            .HasForeignKey(pt => pt.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.Name);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.Property(t => t.Category).IsRequired();

        builder.HasMany(t => t.ProductTags)
            .WithOne(pt => pt.Tag)
            .HasForeignKey(pt => pt.TagId) // Corrected from ProductId to TagId
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.Occasion, t.Category, t.SubCategory }).IsUnique(); // Unique constraint on and indexed by hierarchy of Occasion, Category, SubCategory.
    }
}

public class ProductTagConfiguration : IEntityTypeConfiguration<IXProductTag>
{
    public void Configure(EntityTypeBuilder<IXProductTag> builder)
    {
        builder.HasKey(pt => new {pt.ProductId, pt.TagId});
        builder.ToTable("ProductTags");

        builder.HasOne(pt => pt.Product)
            .WithMany(p => p.ProductTags)
            .HasForeignKey(pt => pt.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(pt => pt.Tag)
            .WithMany(t => t.ProductTags)
            .HasForeignKey(pt => pt.TagId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}