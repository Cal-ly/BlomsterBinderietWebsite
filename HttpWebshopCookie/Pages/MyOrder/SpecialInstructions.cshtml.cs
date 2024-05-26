namespace HttpWebshopCookie.Pages.MyOrder;

[Authorize(Roles = "companyrep")]
public class SpecialInstructionsModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public SpecialInstructionsModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public SpecialOrderInstructionsViewModel SpecialOrderInstructions { get; set; } = new SpecialOrderInstructionsViewModel();

    public async Task<IActionResult> OnGetAsync(string orderId)
    {
        var order = await _context.Orders
            .Include(o => o.SpecialOrderInstruction)
            .ThenInclude(soi => soi!.SpecialDeliveryAddress)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null)
        {
            return NotFound();
        }

        SpecialOrderInstructions = new SpecialOrderInstructionsViewModel
        {
            OrderId = order.Id,
            SpecialInstructions = order.SpecialOrderInstruction?.SpecialInstructions,
            Delivery = order.SpecialOrderInstruction?.Delivery ?? false,
            Arrangement = order.SpecialOrderInstruction?.Arrangement ?? false,
            SpecialDeliveryAddress = new AddressViewModel
            {
                Resident = order.SpecialOrderInstruction?.SpecialDeliveryAddress?.Resident,
                Street = order.SpecialOrderInstruction?.SpecialDeliveryAddress?.Street,
                PostalCode = order.SpecialOrderInstruction?.SpecialDeliveryAddress?.PostalCode,
                City = order.SpecialOrderInstruction?.SpecialDeliveryAddress?.City,
                Country = order.SpecialOrderInstruction?.SpecialDeliveryAddress?.Country
            }
        };

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var order = await _context.Orders
            .Include(o => o.SpecialOrderInstruction)
            .ThenInclude(soi => soi!.SpecialDeliveryAddress)
            .FirstOrDefaultAsync(o => o.Id == SpecialOrderInstructions.OrderId);

        if (order == null)
        {
            return NotFound();
        }

        if (order.SpecialOrderInstruction == null)
        {
            var specialDeliveryAddress = new Address
            {
                Resident = SpecialOrderInstructions.SpecialDeliveryAddress.Resident,
                Street = SpecialOrderInstructions.SpecialDeliveryAddress.Street,
                PostalCode = SpecialOrderInstructions.SpecialDeliveryAddress.PostalCode,
                City = SpecialOrderInstructions.SpecialDeliveryAddress.City,
                Country = SpecialOrderInstructions.SpecialDeliveryAddress.Country
            };

            order.SpecialOrderInstruction = new SpecialOrderInstruction
            {
                OrderId = order.Id,
                SpecialInstructions = SpecialOrderInstructions.SpecialInstructions,
                Delivery = SpecialOrderInstructions.Delivery,
                Arrangement = SpecialOrderInstructions.Arrangement,
                SpecialDeliveryAddress = specialDeliveryAddress
            };
            _context.SpecialOrderInstructions.Add(order.SpecialOrderInstruction);
            _context.Addresses.Add(specialDeliveryAddress);
        }
        else
        {
            var specialOrderInstructions = order.SpecialOrderInstruction;
            var specialDeliveryAddress = specialOrderInstructions.SpecialDeliveryAddress ?? new Address();

            specialDeliveryAddress.Resident = SpecialOrderInstructions.SpecialDeliveryAddress.Resident;
            specialDeliveryAddress.Street = SpecialOrderInstructions.SpecialDeliveryAddress.Street;
            specialDeliveryAddress.PostalCode = SpecialOrderInstructions.SpecialDeliveryAddress.PostalCode;
            specialDeliveryAddress.City = SpecialOrderInstructions.SpecialDeliveryAddress.City;
            specialDeliveryAddress.Country = SpecialOrderInstructions.SpecialDeliveryAddress.Country;

            specialOrderInstructions.SpecialInstructions = SpecialOrderInstructions.SpecialInstructions;
            specialOrderInstructions.Delivery = SpecialOrderInstructions.Delivery;
            specialOrderInstructions.Arrangement = SpecialOrderInstructions.Arrangement;
            specialOrderInstructions.SpecialDeliveryAddress = specialDeliveryAddress;

            _context.SpecialOrderInstructions.Update(specialOrderInstructions);
            _context.Addresses.Update(specialDeliveryAddress);
        }

        await _context.SaveChangesAsync();

        return RedirectToPage("/MyOrder/MyOrderDetails", new { id = order.Id });
    }
}

public class SpecialOrderInstructionsViewModel
{
    public string? OrderId { get; set; }
    public string? SpecialInstructions { get; set; }
    public bool Delivery { get; set; }
    public bool Arrangement { get; set; }
    public AddressViewModel SpecialDeliveryAddress { get; set; } = new AddressViewModel();
}

public class AddressViewModel
{
    public string? Resident { get; set; }
    public string? Street { get; set; }
    public string? PostalCode { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
}