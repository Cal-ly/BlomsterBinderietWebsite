namespace HttpWebshopCookie.Models;

public class SpecialOrderInstruction
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? OrderId { get; set; }
    public Order? Order { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool Delivery { get; set; }
    public bool Arrangement { get; set; }
    public string? SpecialDeliveryAddressId { get; set; }
    public Address? SpecialDeliveryAddress { get; set; }
}
