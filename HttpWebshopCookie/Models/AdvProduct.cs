namespace HttpWebshopCookie.Models;

public class AdvProduct
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? ImageUrl { get; set; }
    public List<AdvProductTag> AdvProductTags { get; set; } = new List<AdvProductTag>();
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