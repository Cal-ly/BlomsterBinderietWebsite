// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

namespace HttpWebshopCookie.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly UserManager<Customer> _customerManager;
        private readonly UserManager<Employee> _employeeManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly SignInManager<Customer> _signInManagerCustomer;
        private readonly SignInManager<Employee> _signInManagerEmployee;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            UserManager<Customer> customerManager,
            UserManager<Employee> employeeManager,
            SignInManager<ApplicationUser> signInManager,
            SignInManager<Customer> signInManagerCustomer,
            SignInManager<Employee> signInManagerEmployee,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _customerManager = customerManager;
            _employeeManager = employeeManager;
            _signInManager = signInManager;
            _signInManagerCustomer = signInManagerCustomer;
            _signInManagerEmployee = signInManagerEmployee;
            _context = context;
        }

        public string Username { get; set; }
        [BindProperty]
        public bool IsCustomer { get; set; } = false;
        [BindProperty]
        public bool IsEmployee { get; set; } = false;

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public Address AddressInput { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Title")]
            public string Title { get; set; }

            [Display(Name = "Birth Date")]
            public DateTime? BirthDate { get; set; }

            [Display(Name = "Job Title")]
            public string JobTitle { get; set; }

            [Display(Name = "Salary")]
            public decimal? Salary { get; set; }
        }

        public class AddressInputModel
        {
            [Display(Name = "Resident")]
            public string Resident { get; set; }

            [Display(Name = "Street")]
            public string Street { get; set; }

            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user) //TODO make generic
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            if (user is Customer customer)
            {
                customer = await _customerManager.FindByNameAsync(userName) ?? customer;
                Input = new InputModel
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    PhoneNumber = phoneNumber,
                    Title = customer.Title,
                    BirthDate = customer.BirthDate
                };
                IsCustomer = true;

            }
            else if (user is Employee employee)
            {
                employee = await _employeeManager.FindByNameAsync(userName) ?? employee;
                Input = new InputModel
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                    PhoneNumber = phoneNumber,
                    JobTitle = employee.JobTitle,
                    Salary = employee.Salary
                };
                IsEmployee = true;
            }
            if (user.AddressId is null || _context.Addresses.Find(user.AddressId) is null)
            {
                AddressInput = new Address()
                {
                    Resident = string.Empty,
                    Street = string.Empty,
                    PostalCode = string.Empty,
                    City = string.Empty,
                    Country = "Denmark"
                };
            }
            else
            {
                AddressInput = _context.Addresses.Find(user.AddressId);
                AddressInput.Resident = $"{user.FirstName} {user.LastName}";
                AddressInput.Street = user.Address.Street;
                AddressInput.PostalCode = user.Address.PostalCode;
                AddressInput.City = user.Address.City;
                AddressInput.Country = user.Address.Country;
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (user is Customer)
            {
                ModelState.Remove("Input.JobTitle");
                ModelState.Remove("Input.Salary");
            }
            else if (user is Employee)
            {
                ModelState.Remove("Input.Title");
                ModelState.Remove("Input.BirthDate");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            if (user is Customer customer)
            {
                await UpdateUserPropertyAsync(customer, customer.FirstName, Input.FirstName, (u, v) => { u.FirstName = v; return _userManager.UpdateAsync(u); });
                await UpdateUserPropertyAsync(customer, customer.LastName, Input.LastName, (u, v) => { u.LastName = v; return _userManager.UpdateAsync(u); });
                await UpdateUserPropertyAsync(customer, customer.PhoneNumber, Input.PhoneNumber, (u, v) => { u.PhoneNumber = v; return _userManager.UpdateAsync(u); });
                await UpdateCustomerPropertyAsync(customer, customer.Title, Input.Title, (u, v) => { u.Title = v; return _customerManager.UpdateAsync(u); });
                await UpdateCustomerPropertyAsync(customer, customer.BirthDate, Input.BirthDate, (u, v) => { u.BirthDate = v; return _customerManager.UpdateAsync(u); });
            }
            else if (user is Employee employee)
            {
                await UpdateUserPropertyAsync(employee, employee.FirstName, Input.FirstName, (u, v) => { u.FirstName = v; return _userManager.UpdateAsync(u); });
                await UpdateUserPropertyAsync(employee, employee.LastName, Input.LastName, (u, v) => { u.LastName = v; return _userManager.UpdateAsync(u); });
                await UpdateUserPropertyAsync(employee, employee.PhoneNumber, Input.PhoneNumber, (u, v) => { u.PhoneNumber = v; return _userManager.UpdateAsync(u); });
                await UpdateEmployeePropertyAsync(employee, employee.JobTitle, Input.JobTitle, (u, v) => { u.JobTitle = v; return _employeeManager.UpdateAsync(u); });
                await UpdateEmployeePropertyAsync(employee, employee.Salary, Input.Salary, (u, v) => { u.Salary = v; return _employeeManager.UpdateAsync(u); });
            }

            if (user.Address == null)
            {
                user.Address = new Address();
                _context.Addresses.Add(user.Address);
            }
            UpdateAddressProperty(user.Address, user.Address.Resident, AddressInput.Resident, (a, v) => a.Street = v);
            UpdateAddressProperty(user.Address, user.Address.Street, AddressInput.Street, (a, v) => a.Street = v);
            UpdateAddressProperty(user.Address, user.Address.PostalCode, AddressInput.PostalCode, (a, v) => a.PostalCode = v);
            UpdateAddressProperty(user.Address, user.Address.City, AddressInput.City, (a, v) => a.City = v);
            UpdateAddressProperty(user.Address, user.Address.Country, AddressInput.Country, (a, v) => a.Country = v);

            _context.SaveChanges();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        private async Task<IdentityResult> UpdateUserPropertyAsync<TUser, TValue>(TUser user, TValue currentValue, TValue newValue, Func<TUser, TValue, Task<IdentityResult>> updateFunc)
            where TUser : ApplicationUser
        {
            if (!EqualityComparer<TValue>.Default.Equals(currentValue, newValue))
            {
                var result = await updateFunc(user, newValue);
                if (!result.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to update property.";
                }
                return result;
            }
            return IdentityResult.Success;
        }

        private async Task<IdentityResult> UpdateCustomerPropertyAsync<TUser, TValue>(TUser user, TValue currentValue, TValue newValue, Func<TUser, TValue, Task<IdentityResult>> updateFunc)
            where TUser : Customer
        {
            if (!EqualityComparer<TValue>.Default.Equals(currentValue, newValue))
            {
                var result = await updateFunc(user, newValue);
                if (!result.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to update property.";
                }
                return result;
            }
            return IdentityResult.Success;
        }

        private async Task<IdentityResult> UpdateEmployeePropertyAsync<TUser, TValue>(TUser user, TValue currentValue, TValue newValue, Func<TUser, TValue, Task<IdentityResult>> updateFunc)
            where TUser : Employee
        {
            if (!EqualityComparer<TValue>.Default.Equals(currentValue, newValue))
            {
                var result = await updateFunc(user, newValue);
                if (!result.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to update property.";
                }
                return result;
            }
            return IdentityResult.Success;
        }

        private static void UpdateAddressProperty<T>(Address address, T currentValue, T newValue, Action<Address, T> updateAction)
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                updateAction(address, newValue);
            }
        }
    }
}