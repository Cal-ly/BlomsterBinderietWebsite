namespace HttpWebshopCookie.Config;

public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);
        builder.ToTable("Companies");
        builder.HasIndex(c => c.CVR).IsUnique();
        builder.HasMany(c => c.Representatives)
            .WithOne(cu => cu.Company)
            .HasForeignKey(cu => cu.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(c => c.Address)
            .WithOne()
            .HasForeignKey<Company>(c => c.AddressId);
    }
}