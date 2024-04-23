namespace HttpWebshopCookie.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.ToTable("Orders");
        builder.Property(o => o.OrderDate)
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETDATE()");
        builder.Property(o => o.CompletionDate)
            .HasColumnType("datetime");
        builder.Property(o => o.Status)
            .HasDefaultValue(OrderStatus.Received)
            .HasConversion<string>();
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.Employee)
            .WithMany(e => e.Orders)
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(o => o.GuestUser)
            .WithMany(g => g.Orders)
            .HasForeignKey(o => o.GuestUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(oi => new {oi.OrderId, oi.ProductId});
        builder.ToTable("OrderItems");
        builder.Property(oi => oi.Quantity)
            .HasDefaultValue(1);
        builder.Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)");
        builder.HasOne(oi => oi.ProductItem)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}