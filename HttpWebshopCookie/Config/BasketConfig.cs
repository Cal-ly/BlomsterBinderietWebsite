namespace HttpWebshopCookie.Config;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        builder.ToTable("Baskets");

        builder.HasMany(b => b.Items)
            .WithOne(i => i.Basket)
            .HasForeignKey(i => i.BasketId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}

public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.HasKey(bi => new {bi.BasketId, bi.ProductId});
        builder.ToTable("BasketItems");

        builder.Property(bi => bi.Quantity).HasDefaultValue(1);

        builder.HasOne(bi => bi.ProductInBasket)
            .WithMany()
            .HasForeignKey(bi => bi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(bi => bi.Basket)
            .WithMany(b => b.Items)
            .HasForeignKey(bi => bi.BasketId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasIndex(bi => bi.BasketId).HasDatabaseName("IDX_BasketId");
        builder.HasIndex(bi => bi.ProductId).HasDatabaseName("IDX_ProductId");
    }
}

public class BasketActivityConfiguration : IEntityTypeConfiguration<BasketActivity>
{
    public void Configure(EntityTypeBuilder<BasketActivity> builder)
    {
        builder.HasKey(ba => ba.Id);
        builder.Property(ba => ba.Id).ValueGeneratedOnAdd();
        builder.ToTable("BasketActivities");

        builder.Property(ba => ba.ActivityType).HasMaxLength(50);
        builder.Property(ba => ba.QuantityChanged).HasDefaultValue(1);
        builder.Property(ba => ba.UserId).HasMaxLength(450);
        builder.Property(ba => ba.IsRegisteredUser).HasDefaultValue(false);
        builder.Property(ba => ba.SessionId).HasMaxLength(450);
        builder.Property(ba => ba.Timestamp).HasColumnType("datetime").HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(ba => ba.Basket)
            .WithMany()
            .HasForeignKey(ba => ba.BasketId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ba => ba.Product)
            .WithMany()
            .HasForeignKey(ba => ba.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(ba => ba.BasketId).HasDatabaseName("IDX_BasketActivity_BasketId");
        builder.HasIndex(ba => ba.ProductId).HasDatabaseName("IDX_BasketActivity_ProductId");
        builder.HasIndex(ba => ba.UserId).HasDatabaseName("IDX_BasketActivity_UserId");
        builder.HasIndex(ba => ba.Timestamp).HasDatabaseName("IDX_BasketActivity_Timestamp");
    }
}