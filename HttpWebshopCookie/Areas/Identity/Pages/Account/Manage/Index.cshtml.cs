// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using HttpWebshopCookie.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;

namespace HttpWebshopCookie.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<Customer> _userManager;
        private readonly SignInManager<Customer> _signInManager;
        private readonly ApplicationDbContext _context;

        public IndexModel(
            //UserManager<ApplicationUser> userManager,
            //SignInManager<ApplicationUser> signInManager,
            UserManager<Customer> userManager,
            SignInManager<Customer> signInManager,
            ApplicationDbContext context)
        {
            //_userManager = userManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }
        
        [BindProperty]
        public Address AddressInput { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
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
        }
        public class AddressInputModel
        {
            [Display(Name = "Street")]
            public string Street { get; set; }

            [Display(Name = "Postal Code")]
            public string PostalCode { get; set; }

            [Display(Name = "City")]
            public string City { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }
        }

        private async Task LoadAsync(Customer user) //TODO make generic
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = phoneNumber,
                Title = user.Title,
                BirthDate = user.BirthDate
            };
            if (user.AddressId is null || _context.Addresses.Find(user.AddressId) is null)
            {
                AddressInput = new Address()
                {
                    Street = string.Empty,
                    PostalCode = string.Empty,
                    City = string.Empty,
                    Country = "Denmark"
                };
            }
            else
            {
                AddressInput = _context.Addresses.Find(user.AddressId);
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

            await UpdateUserPropertyAsync(user, user.FirstName, Input.FirstName, (u, v) => { u.FirstName = v; return _userManager.UpdateAsync(u); });
            await UpdateUserPropertyAsync(user, user.LastName, Input.LastName, (u, v) => { u.LastName = v; return _userManager.UpdateAsync(u); });
            await UpdateUserPropertyAsync(user, user.Title, Input.Title, (u, v) => { u.Title = v; return _userManager.UpdateAsync(u); });
            await UpdateUserPropertyAsync(user, user.BirthDate, Input.BirthDate, (u, v) => { u.BirthDate = v; return _userManager.UpdateAsync(u); });
            await UpdateUserPropertyAsync(user, user.PhoneNumber, Input.PhoneNumber, (u, v) => { u.PhoneNumber = v; return _userManager.UpdateAsync(u); });

            if (user.Address == null)
            {
                user.Address = new Address();
                _context.Addresses.Add(user.Address);
            }
            UpdateAddressProperty(user.Address, user.Address.Street, AddressInput.Street, (a, v) => a.Street = v);
            UpdateAddressProperty(user.Address, user.Address.Street, AddressInput.Street, (a, v) => a.Street = v);
            UpdateAddressProperty(user.Address, user.Address.PostalCode, AddressInput.PostalCode, (a, v) => a.PostalCode = v);
            UpdateAddressProperty(user.Address, user.Address.City, AddressInput.City, (a, v) => a.City = v);
            UpdateAddressProperty(user.Address, user.Address.Country, AddressInput.Country, (a, v) => a.Country = v);

            _context.SaveChanges();

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
        private async Task<IdentityResult> UpdateUserPropertyAsync<T>(Customer user, T currentValue, T newValue, Func<Customer, T, Task<IdentityResult>> updateFunc)
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                var result = await updateFunc(user, newValue);
                if (!result.Succeeded)
                {
                    StatusMessage = $"Unexpected error when trying to update property.";
                }
                return result;
            }
            return IdentityResult.Success;
        }
        private void UpdateAddressProperty<T>(Address address, T currentValue, T newValue, Action<Address, T> updateAction)
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, newValue))
            {
                updateAction(address, newValue);
            }
        }
    }
}