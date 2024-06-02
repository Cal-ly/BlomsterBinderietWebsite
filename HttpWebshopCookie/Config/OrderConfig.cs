namespace HttpWebshopCookie.Config;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedOnAdd();

        builder.ToTable("Orders");
        builder.Property(o => o.OrderDate)
            .HasColumnType("datetime")
            .HasDefaultValue(DateTime.UtcNow);
        builder.Property(o => o.CompletionDate)
            .HasColumnType("datetime");
        builder.Property(o => o.Status)
            .HasDefaultValue(OrderStatus.Pending);
        builder.Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(o => o.Guest)
            .WithMany(g => g.Orders)
            .HasForeignKey(o => o.GuestId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasOne(o => o.Employee)
            .WithMany(e => e.Orders)
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.SetNull);
        builder.HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(o => o.SpecialOrderInstruction)
            .WithOne(si => si.Order)
            .HasForeignKey<SpecialOrderInstruction>(si => si.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(o => o.OrderDate).HasDatabaseName("IDX_OrderDate").IsDescending();
        builder.HasIndex(o => o.CustomerId).HasDatabaseName("IDX_CustomerId");
        builder.HasIndex(o => o.GuestId).HasDatabaseName("IDX_GuestId");
        builder.HasIndex(o => o.EmployeeId).HasDatabaseName("IDX_EmployeeId");
        builder.HasIndex(o => o.Status).HasDatabaseName("IDX_Status");
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
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SpecialOrderInstructionsConfiguration : IEntityTypeConfiguration<SpecialOrderInstruction>
{
    public void Configure(EntityTypeBuilder<SpecialOrderInstruction> builder)
    {
        builder.HasKey(soi => soi.Id);
        builder.Property(soi => soi.Id).ValueGeneratedOnAdd();

        builder.ToTable("SpecialOrderInstructions");

        builder.HasOne(soi => soi.Order)
            .WithOne(o => o.SpecialOrderInstruction)
            .HasForeignKey<SpecialOrderInstruction>(soi => soi.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}