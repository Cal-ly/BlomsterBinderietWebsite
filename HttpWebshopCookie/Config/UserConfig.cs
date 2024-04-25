namespace HttpWebshopCookie.Config;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        builder.ToTable("Customers");
        builder.HasIndex(c => c.Email).IsUnique();
        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Address)
            .WithOne()
            .HasForeignKey<Customer>(c => c.AddressId)
            .OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne(c => c.Company)
            .WithMany(c => c.Representatives)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);
        builder.ToTable("Employees");
        builder.HasIndex(e => e.Email).IsUnique();
        builder.Property(e => e.Salary).HasColumnType("decimal(18,2)");
        builder.HasMany(e => e.Orders)
            .WithOne(e => e.Employee)
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.Address)
            .WithOne()
            .HasForeignKey<Employee>(e => e.AddressId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.HasKey(g => g.Id);
        builder.ToTable("Guests");
        builder.HasMany(g => g.Orders)
            .WithOne(o => o.Guest)
            .HasForeignKey(o => o.GuestId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(g => g.Address)
            .WithOne()
            .HasForeignKey<Guest>(g => g.AddressId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}