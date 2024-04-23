namespace HttpWebshopCookie.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.ToTable("Products");
        builder.Property(p => p.Name).IsRequired();
        builder.Property(p => p.Description).IsRequired();
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(p => p.IsActivated).HasColumnType("bool").HasDefaultValue(true);
        builder.Property(p => p.UpdatedAt).HasColumnType("datetime");
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

//AdvTags will cover category, brand, etc.

//Using a join table to connect AdvProducts and AdvTags
//This is a many-to-many relationship
//The join table is called AdvProductTag
//It has two foreign keys, one for AdvProduct and one for AdvTag
//The join table is a separate class, not a DbSet
//The join table has navigation properties to the other two classes
//The join table is used to configure the many-to-many relationship

//Application
//var product = new Product { Name = "Example Product", Description = "Example Description", Price = 19.99m };
//var tag = new Tag { Name = "Electronics" };
//product.ProductTags.Add(new ProductTag { Product = product, Tag = tag });

//Database
//
//  //Define composite primary key
//modelBuilder.Entity<ProductTag>()
//        .HasKey(pt => new { pt.ProductId, pt.TagId });

//  // Configure many-to-many relationship
//modelBuilder.Entity<ProductTag>()
//    .HasOne(pt => pt.Product)
//    .WithMany(p => p.ProductTags)
//    .HasForeignKey(pt => pt.ProductId);

//modelBuilder.Entity<ProductTag>()
//    .HasOne(pt => pt.Tag)
//    .WithMany(t => t.ProductTags)
//    .HasForeignKey(pt => pt.TagId);