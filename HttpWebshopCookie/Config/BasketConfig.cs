namespace HttpWebshopCookie.Config;

public class BasketConfiguration : IEntityTypeConfiguration<Basket>
{
    public void Configure(EntityTypeBuilder<Basket> builder)
    {
        builder.HasKey(b => b.Id);
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
    }
}

public class BasketActivityConfiguration : IEntityTypeConfiguration<BasketActivity>
{
    public void Configure(EntityTypeBuilder<BasketActivity> builder)
    {
        builder.HasKey(ba => ba.Id);
        builder.ToTable("BasketActivities");
        builder.Property(ba => ba.Timestamp).HasColumnType("datetime");
        builder.HasOne(ba => ba.Basket)
            .WithMany()
            .HasForeignKey(ba => ba.BasketId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ba => ba.User)
            .WithMany()
            .HasForeignKey(ba => ba.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(ba => ba.Product)
            .WithMany()
            .HasForeignKey(ba => ba.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}