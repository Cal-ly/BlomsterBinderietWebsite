namespace HttpWebshopCookie.Config;

public class UserConfiguration<T> : IEntityTypeConfiguration<T> where T : ApplicationUser
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(u => u.FirstName).IsRequired();
        builder.Property(u => u.LastName).IsRequired();
        builder.Property(u => u.EnrollmentDate).HasColumnType("datetime").HasDefaultValue(DateTime.UtcNow);
        builder.Property(u => u.LastLogin).HasColumnType("datetime");

        builder.HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<T>(u => u.AddressId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}

public class CustomerConfiguration : UserConfiguration<Customer>
{
    public override void Configure(EntityTypeBuilder<Customer> builder)
    {
        base.Configure(builder);
        builder.ToTable("Customers");
        builder.Property(c => c.BirthDate).HasColumnType("datetime");

        builder.HasMany(c => c.Orders)
            .WithOne(o => o.Customer)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<Customer>(u => u.AddressId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(c => c.Company)
            .WithMany(c => c.Representatives)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
public class EmployeeConfiguration : UserConfiguration<Employee>
{
    public override void Configure(EntityTypeBuilder<Employee> builder)
    {
        base.Configure(builder);
        builder.ToTable("Employees");
        builder.Property(e => e.Salary).HasColumnType("decimal(18,2)");
        builder.Property(e => e.TerminationDate).HasColumnType("datetime");

        builder.HasMany(e => e.Orders)
            .WithOne(e => e.Employee)
            .HasForeignKey(o => o.EmployeeId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<Employee>(u => u.AddressId)
            .OnDelete(DeleteBehavior.ClientCascade);
    }
}

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.ToTable("GuestUsers");

        builder.Property(g => g.Id).ValueGeneratedOnAdd();
        builder.Property(g => g.FirstName).IsRequired();
        builder.Property(g => g.LastName).IsRequired();
        builder.Property(g => g.Email).IsRequired();
        builder.Property(g => g.PhoneNumber).IsRequired();

        builder.HasKey(g => g.Id);
        builder.HasMany(g => g.Orders)
            .WithOne(o => o.Guest)
            .HasForeignKey(o => o.GuestId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(g => g.Address)
            .WithOne()
            .HasForeignKey<Guest>(g => g.AddressId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}