namespace HttpWebshopCookie.Config;

public class UserConfiguration<T> : IEntityTypeConfiguration<T> where T : ApplicationUser
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.Property(u => u.Email)
            .HasComment("Must be a valid email format")
            .IsRequired()
            .HasMaxLength(256);
        //builder.Property(u => u.FirstName).IsRequired();
        //builder.Property(u => u.LastName).IsRequired();
        //builder.Property(u => u.PhoneNumber).IsRequired();
        builder.Property(u => u.EnrollmentDate).HasColumnType("datetime").HasDefaultValue(DateTime.UtcNow);
        builder.Property(u => u.LastLogin).HasColumnType("datetime");

        builder.HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<T>(u => u.AddressId)
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.HasIndex(u => u.Email).IsUnique();
    }
}

public class ApplicationUserConfiguration : UserConfiguration<ApplicationUser>
{
    public override void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        base.Configure(builder);
        builder.ToTable("ApplicationUsers");
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
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Company)
            .WithMany(c => c.Representatives)
            .HasForeignKey(c => c.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
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
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).ValueGeneratedOnAdd();
        builder.ToTable("Guests");

        builder.Property(g => g.FirstName).IsRequired();
        builder.Property(g => g.LastName).IsRequired();
        builder.Property(g => g.Email).IsRequired();
        builder.Property(g => g.PhoneNumber).IsRequired();
        builder.HasKey(g => g.Id);
        builder.HasMany(g => g.Orders)
            .WithOne(o => o.Guest)
            .HasForeignKey(o => o.GuestId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(g => g.Address)
            .WithOne()
            .HasForeignKey<Guest>(g => g.AddressId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(g => g.Email).IsUnique();
    }
}