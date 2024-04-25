namespace HttpWebshopCookie.Interfaces;

public class UserWrapper
{
    private readonly Guest? _guest;
    private readonly Customer? _customer;
    private readonly Employee? _employee;
    private readonly ApplicationUser? _applicationUser;

    public UserWrapper(Guest guestUser)
    {
        _guest = guestUser;
    }
    public UserWrapper(Customer customer)
    {
        _customer = customer;
    }
    public UserWrapper(Employee employee)
    {
        _employee = employee;
    }
    public UserWrapper(ApplicationUser applicationUser)
    {
        _applicationUser = applicationUser;
    }

    public string Id
    {
        get
        {
            if (_guest != null)
            {
                return _guest.Id;
            }
            else if (_customer != null)
            {
                return _customer.Id;
            }
            else if (_employee != null)
            {
                return _employee.Id;
            }
            else if (_applicationUser != null)
            {
                return _applicationUser.Id;
            }
            else
            {
                throw new InvalidOperationException("No user is set");
            }
        }
        set
        {
            if (_guest != null)
            {
                _guest.Id = value;
            }
            else if (_customer != null)
            {
                _customer.Id = value;
            }
            else if (_employee != null)
            {
                _employee.Id = value;
            }
            else if (_applicationUser != null)
            {
                _applicationUser.Id = value;
            }
            else
            {
                throw new InvalidOperationException("No user is set");
            }
        }
    }
    public Guest? Guest => _guest;
    public Customer? Customer => _customer;
    public Employee? Employee => _employee;
    public ApplicationUser? ApplicationUser => _customer;
}