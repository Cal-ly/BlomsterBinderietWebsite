﻿namespace HttpWebshopCookie.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<BasketActivity> BasketActivities { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

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
    }
}