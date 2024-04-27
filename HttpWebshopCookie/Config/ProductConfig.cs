namespace HttpWebshopCookie.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        builder.ToTable("Products");

        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.UpdatedAt).HasColumnType("datetime").HasDefaultValue("GETUTCDATE()");

        builder.HasMany(p => p.ProductTags)
            .WithOne(pt => pt.Product)
            .HasForeignKey(pt => pt.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).ValueGeneratedOnAdd();
        builder.ToTable("Tags");

        builder.Property(t => t.Catergory).IsRequired();

        builder.HasMany(t => t.ProductTags)
            .WithOne(pt => pt.Tag)
            .HasForeignKey(pt => pt.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
{
    public void Configure(EntityTypeBuilder<ProductTag> builder)
    {
        builder.HasKey(pt => new {pt.ProductId, pt.TagId});
        builder.ToTable("ProductTags");

        builder.Property(pt => pt.ProductId).IsRequired();
        builder.Property(pt => pt.TagId).IsRequired();

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