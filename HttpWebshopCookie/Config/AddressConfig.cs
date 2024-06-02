namespace HttpWebshopCookie.Config;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).ValueGeneratedOnAdd();
        builder.ToTable("Addresses");

        builder.Property(a => a.Street).IsRequired();
        builder.Property(a => a.City).IsRequired();
        builder.Property(a => a.PostalCode).IsRequired();
        builder.Property(a => a.Country).IsRequired().HasDefaultValue("Denmark");

        builder.HasIndex(a => new { a.PostalCode, a.Country }).HasDatabaseName("IDX_PostalCode").HasFilter("[Country] = 'Denmark'");
    }
}