namespace HttpWebshopCookie.Interfaces;

public class UserWrapper
{
    public string Id
    {
        get
        {
            if (Guest != null)
            {
                return Guest.Id;
            }
            else if (Customer != null)
            {
                return Customer.Id;
            }
            else if (Employee != null)
            {
                return Employee.Id;
            }
            else if (ApplicationUser != null)
            {
                return ApplicationUser.Id;
            }
            else
            {
                throw new InvalidOperationException("No user is set");
            }
        }
        set
        {
            if (Guest != null)
            {
                Guest.Id = value;
            }
            else if (Customer != null)
            {
                Customer.Id = value;
            }
            else if (Employee != null)
            {
                Employee.Id = value;
            }
            else if (ApplicationUser != null)
            {
                ApplicationUser.Id = value;
            }
            else
            {
                throw new InvalidOperationException("No user is set");
            }
        }
    }
    public string FirstName => Guest?.FirstName ?? Customer?.FirstName ?? Employee?.FirstName ?? ApplicationUser?.FirstName ?? throw new InvalidOperationException("No user is set");
    public string LastName => Guest?.LastName ?? Customer?.LastName ?? Employee?.LastName ?? ApplicationUser?.LastName ?? throw new InvalidOperationException("No user is set");
    public string Email => Guest?.Email ?? Customer?.Email ?? Employee?.Email ?? ApplicationUser?.Email ?? throw new InvalidOperationException("No user is set");
    public string PhoneNumber => Guest?.PhoneNumber ?? Customer?.PhoneNumber ?? Employee?.PhoneNumber ?? ApplicationUser?.PhoneNumber ?? throw new InvalidOperationException("No user is set");
    public Address? Address => Guest?.Address ?? Customer?.Address ?? throw new InvalidOperationException("No user is set");
    public Guest? Guest { get; }
    public Customer? Customer { get; }
    public Employee? Employee { get; }
    public ApplicationUser? ApplicationUser { get; }

    public UserWrapper(Guest guestUser)
    {
        Guest = guestUser;
    }
    public UserWrapper(Customer customer)
    {
        Customer = customer;
    }
    public UserWrapper(Employee employee)
    {
        Employee = employee;
    }
    public UserWrapper(ApplicationUser applicationUser)
    {
        ApplicationUser = applicationUser;
    }

    public string GetUserType()
    {
        if (Guest != null)
        {
            return "Guest";
        }
        else if (Customer != null)
        {
            return "Customer";
        }
        else if (Employee != null)
        {
            return "Employee";
        }
        else if (ApplicationUser != null)
        {
            return "ApplicationUser";
        }
        else
        {
            throw new InvalidOperationException("No user is set");
        }
    }
}