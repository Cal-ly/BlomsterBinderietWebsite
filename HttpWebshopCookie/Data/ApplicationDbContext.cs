using HttpWebshopCookie.Models.Users;
using HttpWebshopCookie.Models.IndexTables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HttpWebshopCookie.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<GuestUser> GuestUsers { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProductTag> ProductTags { get; set; }
    //public DbSet<Product> Products { get; set; }
    //public DbSet<Basket> Baskets { get; set; }
    //public DbSet<BasketItem> BasketItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Customize the ASP.NET Identity model and override the defaults if needed.

        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");
            entity.HasMany(e => e.Orders)
                .WithOne()
                .HasForeignKey(o => o.UserId);
            entity.HasOne(e => e.Address)
                .WithOne()
                .HasForeignKey<ApplicationUser>(e => e.AddressId);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.Property(e => e.Title);
            entity.Property(e => e.BirthDate);
            entity.Property(e => e.RegistrationDate);
            entity.Property(e => e.LastLogin);
            entity.HasOne(e => e.Company)
                .WithMany(c => c.Customers)
                .HasForeignKey(e => e.CompanyId);
            entity.HasMany(e => e.Orders)
                .WithOne()
                .HasForeignKey(o => o.UserId);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.ToTable("Employees");
            entity.Property(e => e.EmployeeNumber);
            entity.Property(e => e.JobTitle);
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.HireDate);
            entity.Property(e => e.TerminationDate);
            entity.HasOne(e => e.Address)
                .WithOne()
                .HasForeignKey<Employee>(e => e.AddressId);
        });

        // Setting up one-to-many relationship between Company and Customers
        modelBuilder.Entity<Company>()
            .HasMany(c => c.Customers)
            .WithOne(e => e.Company)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);  // This prevents cascading deletes






        //Define composite primary key
        modelBuilder.Entity<ProductTag>()
                .HasKey(pt => new { pt.ProductId, pt.TagId });

        // Configure many-to-many relationship
        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Product)
            .WithMany(p => p.ProductTags)
            .HasForeignKey(pt => pt.ProductId);

        modelBuilder.Entity<ProductTag>()
            .HasOne(pt => pt.Tag)
            .WithMany(t => t.ProductTags)
            .HasForeignKey(pt => pt.TagId);

        /// <summary>
        /// Product configuration. Product has a name, description and price
        /// </summary>
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.ToTable("Products");
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Name);
            entity.Property(p => p.Description);
        });

        /// <summary>
        /// Basket configuration. Basket has many BasketItems with a foreign key BasketId
        /// On delete cascade, which means if a basket is deleted, all items in the basket are deleted
        /// </summary>
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.ToTable("Baskets");
            entity.HasKey(p => p.Id);
            entity.HasMany(p => p.Items)
                .WithOne(p => p.Basket)
                .HasForeignKey(p => p.BasketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        /// <summary>
        /// BasketItem configuration. BasketItem has a quantity, a foreign key to a product and a foreign key to a basket
        /// Delete behavior is restrict, which means that if a product is deleted, the basket item is not deleted
        /// Delete behavior is cascade, which means that if a basket is deleted, the basket item is deleted
        /// </summary>
        modelBuilder.Entity<BasketItem>(entity =>
        {
            entity.HasKey(p => new { p.BasketId, p.ProductId }); // Composite key
            entity.HasOne(p => p.ProductInBasket)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(p => p.Basket)
                .WithMany(p => p.Items)
                .HasForeignKey(p => p.BasketId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(p => p.Quantity).HasDefaultValue(1);
        });
    }
}
