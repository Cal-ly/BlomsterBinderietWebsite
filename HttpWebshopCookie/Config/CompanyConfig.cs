namespace HttpWebshopCookie.Config;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        builder.ToTable("Companies");

        builder.HasOne(c => c.Address)
            .WithOne()
            .HasForeignKey<Company>(c => c.AddressId);
        builder.HasMany(c => c.Representatives)
            .WithOne(cu => cu.Company)
            .HasForeignKey(cu => cu.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => c.CVR).IsUnique();
    }
}