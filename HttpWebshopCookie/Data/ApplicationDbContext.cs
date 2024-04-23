using HttpWebshopCookie.Models.Users;
using HttpWebshopCookie.Models.IndexTables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace HttpWebshopCookie.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<GuestUser> GuestUsers { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<BasketActivity> BasketActivities { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProductTag> ProductTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Customers");
            entity.Property(e => e.FirstName);
            entity.Property(e => e.LastName);
            entity.Property(e => e.Title);
            entity.Property(e => e.BirthDate);
            entity.Property(e => e.RegistrationDate);
            entity.Property(e => e.LastLogin);
            entity.HasOne(e => e.Company)
                .WithMany(c => c.Customers)
                .HasForeignKey(e => e.CompanyId);
            entity.HasMany(e => e.Orders)
                .WithOne()
                .HasForeignKey(o => o.CustomerId);
            entity.HasOne(e => e.Address)
                .WithOne()
                .HasForeignKey<Customer>(e => e.AddressId);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Employees");
            entity.Property(e => e.FirstName);
            entity.Property(e => e.LastName);
            entity.Property(e => e.EmployeeNumber);
            entity.Property(e => e.JobTitle);
            entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.HireDate);
            entity.Property(e => e.TerminationDate);
            entity.HasMany(e => e.Orders)
                .WithOne()
                .HasForeignKey(o => o.EmployeeId);
            entity.HasOne(e => e.Address)
                .WithOne()
                .HasForeignKey<Employee>(e => e.AddressId);
        });

        modelBuilder.Entity<GuestUser>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("GuestUsers");
            entity.Property(e => e.Email);
            entity.Property(e => e.Phone);
            entity.HasOne(e => e.Address)
                .WithOne()
                .HasForeignKey<GuestUser>(e => e.AddressId);
            entity.HasMany(e => e.Orders)
                .WithOne()
                .HasForeignKey(o => o.GuestUserId);
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Addresses");
            entity.Property(e => e.Resident);
            entity.Property(e => e.Street);
            entity.Property(e => e.City);
            entity.Property(e => e.PostalCode);
            entity.Property(e => e.Country).HasDefaultValue("Denmark");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Companies");
            entity.HasMany(c => c.Customers)
                .WithOne(e => e.Company)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(c => c.Address)
                .WithOne()
                .HasForeignKey<Company>(c => c.AddressId);
        });

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
            entity.HasMany(p => p.ProductTags)
                .WithOne(pt => pt.Product)
                .HasForeignKey(pt => pt.ProductId);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Orders");
            entity.Property(e => e.OrderDate);
            entity.Property(e => e.CompletionDate);
            entity.Property(e => e.Status);
            entity.HasMany(e => e.OrderItems)
                .WithOne()
                .HasForeignKey(oi => oi.OrderId);
            entity.HasOne(e => e.Employee)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.EmployeeId);
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("OrderItems");
            entity.Property(e => e.Quantity);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                .WithMany()
                .HasForeignKey(e => e.ProductId);
            entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(e => e.OrderId);
        });

        /// <summary>
        /// Basket configuration. Basket has many BasketItems with a foreign key BasketId
        /// On delete cascade, which means if a basket is deleted, all items in the basket are deleted
        /// </summary>
        modelBuilder.Entity<Basket>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.ToTable("Baskets");
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

        /// <summary>
        /// BasketActivity configuration. BasketActivity has a session ID, product ID, quantity, activity type and timestamp
        /// </summary>
        modelBuilder.Entity<BasketActivity>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.ToTable("BasketActivities");
            entity.Property(p => p.BasketId);
            entity.Property(p => p.ProductId);
            entity.Property(p => p.UserId);
            entity.Property(p => p.SessionId);
            entity.Property(p => p.ActivityType);
            entity.Property(p => p.QuantityChanged);
            entity.Property(p => p.Timestamp).HasColumnType("datetime");
            entity.HasOne(p => p.Basket)
                .WithMany()
                .HasForeignKey(p => p.BasketId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(p => p.Product)
                .WithMany()
                .HasForeignKey(p => p.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(t => t.Id);
            entity.ToTable("Tags");
            entity.Property(t => t.Catergory);
            entity.Property(t => t.SubCategory);
            entity.HasMany(t => t.ProductTags)
                .WithOne(pt => pt.Tag)
                .HasForeignKey(pt => pt.TagId);
        });

        modelBuilder.Entity<ProductTag>(entity =>
        {
            entity.HasKey(pt => new { pt.ProductId, pt.TagId });
            entity.ToTable("ProductTags");
            entity.HasOne(pt => pt.Product)
                .WithMany(p => p.ProductTags)
                .HasForeignKey(pt => pt.ProductId);
            entity.HasOne(pt => pt.Tag)
                .WithMany(t => t.ProductTags)
                .HasForeignKey(pt => pt.TagId);
        });
    }
}
