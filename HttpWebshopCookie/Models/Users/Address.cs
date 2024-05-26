namespace HttpWebshopCookie.Models.Users;

public class Address
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Resident { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public string? FullAddress()
    {
        System.Text.StringBuilder fullAddress = new System.Text.StringBuilder();

        if (!string.IsNullOrEmpty(Resident))
        {
            fullAddress.Append(Resident).Append(", ");
        }
        if (!string.IsNullOrEmpty(Street))
        {
            fullAddress.Append(Street).Append(", ");
        }
        if (!string.IsNullOrEmpty(City))
        {
            fullAddress.Append(City).Append(", ");
        }
        if (!string.IsNullOrEmpty(PostalCode))
        {
            fullAddress.Append(PostalCode).Append(", ");
        }
        if (!string.IsNullOrEmpty(Country))
        {
            fullAddress.Append(Country);
        }

        return fullAddress.ToString();
    }
}