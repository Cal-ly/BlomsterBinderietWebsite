using HttpWebshopCookie.Models.Users;
using Microsoft.AspNetCore.Identity;

namespace HttpWebshopCookie.Data.MockData;

public class SeedUsers(IServiceProvider serviceProvider)
{
    private readonly ApplicationDbContext context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    private readonly UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    private static readonly Random random = new();
    private const string Password = "Tester";

    public List<string>? EmployeeIdList = [];   
    public List<string>? CompanyRepIdList = [];
    public List<string>? CustomerIdList = [];
    public List<string>? GuestIdList = [];

    public void SeedEmployee()
    {
        List<Address> employeeAddressList = new List<Address>();
        List<Employee> employeeList = new List<Employee>();

        string[] EmployeeRoles = ["admin", "manager", "staff", "assistant"];
        for(int i = 0; i < EmployeeRoles.Length; i++)
        {
            string[] randomAddress = GenerateRandomAddress(random);
            string[] randomName = GenerateRandomName(random);
            Address employeeAddress = new()
            {
                Resident = $"{randomName[0]} {randomName[1]}",
                Street = $"{randomAddress[0]}",
                PostalCode = $"{randomAddress[1]}",
                City = $"{randomAddress[2]}",
            };
            Employee employeeUser = new()
            {
                UserName = $"{EmployeeRoles[i]}@test.com",
                NormalizedUserName = EmployeeRoles[i].ToUpper(),
                Email = $"{EmployeeRoles[i]}@test.com",
                NormalizedEmail = $"{EmployeeRoles[i]}@test.com".ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = random.Next(10000000, 99999999).ToString(),
                PhoneNumberConfirmed = true,
                SecurityStamp = string.Empty,
                FirstName = randomName[0],
                LastName = randomName[1],
                JobTitle = EmployeeRoles[i],
                Salary = decimal.Parse(random.Next(170000, 500000).ToString()),
                EnrollmentDate = DateTime.UtcNow,
                AddressId = employeeAddress.Id,
                Address = employeeAddress
            };
            EmployeeIdList?.Add(employeeUser.Id);
            employeeAddressList.Add(employeeAddress);
            employeeList.Add(employeeUser);
        }
        context.Addresses.AddRange(employeeAddressList);
        context.SaveChanges();
        foreach (var employee in employeeList)
        {
            userManager.CreateAsync(employee, Password).Wait();
            userManager.AddToRoleAsync(employee, employee.JobTitle!).Wait();
        }
    }

    public void SeedCompanies()
    {
        List<Address> companyAddresses = [];
        List<Company> companies = [];
        List<Customer> companyReps = [];
        for (int i = 0; i < 6; i++)
        {
            string randomCVR = random.Next(10000000, 99999999).ToString();
            string randomPhone = random.Next(10000000, 99999999).ToString();
            string[] randomAddress = GenerateRandomAddress(random);
            string[] randomName = GenerateRandomName(random);
            string uniqueEmail = $"{randomName[0]}{randomName[1]}{random.Next(10, 99)}" + "rep@test.com";
            Address companyAddress = new()
            {
                Resident = $"Company {i}",
                Street = $"{randomAddress[0]}",
                PostalCode = $"{randomAddress[1]}",
                City = $"{randomAddress[2]}",
            };
            companyAddresses.Add(companyAddress);

            Company company = new()
            {
                CVR = randomCVR,
                Name = $"Company {i}",
                PhoneNumber = randomPhone,
                AddressId = companyAddress.Id
            };
            companies.Add(company);

            Customer companyRep = new()
            {
                UserName = uniqueEmail,
                NormalizedUserName = uniqueEmail.ToUpper(),
                Email = uniqueEmail,
                NormalizedEmail = uniqueEmail.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = randomPhone,
                FirstName = randomName[0],
                LastName = randomName[1],
                AddressId = companyAddress.Id,
                Address = companyAddress,
                Title = "Owner",
                BirthDate = DateTime.UtcNow.AddYears(-random.Next(18, 70)),
                CompanyId = company.Id
            };
            companyReps.Add(companyRep);
            CompanyRepIdList?.Add(companyRep.Id);
        }
        context.Addresses.AddRange(companyAddresses);
        context.Companies.AddRange(companies);
        context.SaveChanges();
        foreach (var companyRep in companyReps)
        {
            userManager.CreateAsync(companyRep, Password).Wait();
            userManager.AddToRoleAsync(companyRep, "companyrep").Wait();
        }
    }

    public void SeedCustomers()
    {
        List<Address> customerAddresses = [];
        List<Customer> customers = [];
        for (int i = 0; i < 10; i++)
        {
            string[] randomAddress = GenerateRandomAddress(random);
            string[] randomName = GenerateRandomName(random);
            string uniqueEmail = $"{randomName[0]}{randomName[1]}{random.Next(10, 99)}@test.com";
            string randomPhone = random.Next(10000000, 99999999).ToString();
            Address customerAddress = new()
            {
                Resident = $"{randomName[0]} {randomName[1]}",
                Street = $"{randomAddress[0]}",
                PostalCode = $"{randomAddress[1]}",
                City = $"{randomAddress[2]}",
            };
            customerAddresses.Add(customerAddress);

            Customer customer = new()
            {
                UserName = uniqueEmail,
                NormalizedUserName = uniqueEmail.ToUpper(),
                Email = uniqueEmail,
                NormalizedEmail = uniqueEmail.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = randomPhone,
                FirstName = randomName[0],
                LastName = randomName[1],
                AddressId = customerAddress.Id, //implement address property
                Address = customerAddress,
                Title = "Mr/Mrs/Ms",
                BirthDate = DateTime.UtcNow.AddYears(-random.Next(18, 70)) 
            };
            customers.Add(customer);
            CustomerIdList?.Add(customer.Id);
        }

        context.Addresses.AddRange(customerAddresses);
        context.SaveChanges();
        foreach (var customer in customers)
        {
            userManager.CreateAsync(customer, Password).Wait();
            userManager.AddToRoleAsync(customer, "customer").Wait();
        }
    }

    public void SeedTestAdmin()
    {
        string uniqueEmail = "admin@admin.com";
        string[] randomName = ["Admin", "Adminson"];
        string[] randomAddress = GenerateRandomAddress(random);
        string randomPhone = random.Next(10000000, 99999999).ToString();
        Address adminAddress = new()
        {
            Resident = "Admin Adminson",
            Street = $"{randomAddress[0]}",
            PostalCode = $"{randomAddress[1]}",
            City = $"{randomAddress[2]}",
        };

        Employee admin = new()
        {
            UserName = "admin@admin.com",
            NormalizedUserName = uniqueEmail.ToUpper(),
            Email = uniqueEmail,
            NormalizedEmail = uniqueEmail.ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            PhoneNumber = randomPhone,
            FirstName = randomName[0],
            LastName = randomName[1],
            JobTitle = "TestAdmin",
            Salary = decimal.Parse(random.Next(170000, 500000).ToString()),
            EnrollmentDate = DateTime.UtcNow,
            AddressId = adminAddress.Id,
            Address = adminAddress,
        };
        EmployeeIdList?.Add(admin.Id);

        context.Addresses.Add(adminAddress);
        context.SaveChanges();
        userManager.CreateAsync(admin, Password).Wait();
        userManager.AddToRoleAsync(admin, "admin").Wait();
    }

    public void SeedTestCustomer()
    {
        string uniqueEmail = "test@test.com";
        string[] randomName = ["Test", "Tester"];
        string[] randomAddress = GenerateRandomAddress(random);
        string randomPhone = random.Next(10000000, 99999999).ToString();
        Address customerAddress = new()
        {
            Resident = "Test Tester",
            Street = $"{randomAddress[0]}",
            PostalCode = $"{randomAddress[1]}",
            City = $"{randomAddress[2]}",
        };

        Customer customer = new()
        {
            UserName = "test@test.com",
            NormalizedUserName = uniqueEmail.ToUpper(),
            Email = uniqueEmail,
            NormalizedEmail = uniqueEmail.ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            PhoneNumber = randomPhone,
            FirstName = randomName[0],
            LastName = randomName[1],
            AddressId = customerAddress.Id,
            Address = customerAddress,
            Title = "DevOps",
            BirthDate = DateTime.UtcNow.AddYears(-random.Next(18, 70))
        };
        CustomerIdList?.Add(customer.Id);

        context.Addresses.Add(customerAddress);
        context.SaveChanges();
        userManager.CreateAsync(customer, Password).Wait();
        userManager.AddToRoleAsync(customer, "customer").Wait();
    }

    public void SeedGuests()
    {
        List<Address> guestAddresses = [];
        List<Guest> guests = [];
        for (int i = 0; i < 10; i++)
        {
            string[] randomAddress = GenerateRandomAddress(random);
            string[] randomName = GenerateRandomName(random);
            string uniqueEmail = $"{randomName[0]}{randomName[1]}{random.Next(10, 99)}@test.com";
            string randomPhone = random.Next(10000000, 99999999).ToString();
            Address guestAddress = new()
            {
                Resident = $"{randomName[0]} {randomName[1]}",
                Street = $"{randomAddress[0]}",
                PostalCode = $"{randomAddress[1]}",
                City = $"{randomAddress[2]}",
            };
            guestAddresses.Add(guestAddress);

            Guest guest = new()
            {
                Email = uniqueEmail,
                FirstName = randomName[0],
                LastName = randomName[1],
                PhoneNumber = randomPhone,
                AddressId = guestAddress.Id
            };
            guests.Add(guest);
            GuestIdList?.Add(guest.Id);
        }

        context.Addresses.AddRange(guestAddresses);
        context.GuestUsers.AddRange(guests);
        context.SaveChanges();
    }

    public static string[] GenerateRandomName(Random random)
    {
        string[] firstNames = ["Hans", "Jens", "Lone", "John", "Emma", "Michael", "Sophia", "William", "Olivia", "Kristian"];
        int indexFirst = random.Next(firstNames.Length);

        string[] lastNames = ["Hansen", "Jensen", "Johnson", "Gormsen", "Michaelson", "Kristiansdottir", "Wilson", "Olivera"];
        int indexLast = random.Next(lastNames.Length);

        return [firstNames[indexFirst], lastNames[indexLast]];
    }

    public static string[] GenerateRandomAddress(Random random)
    {
        string[] streets = ["Hovedgade", "Bakkevej", "Skovvej", "Strandvej", "Søndergade", "Nørregade", "Vestergade", "Østergade"];
        int indexStreet = random.Next(streets.Length);

        string streetCombine;
        string streetNumber = random.Next(1, 99).ToString();
        if (random.Next(1, 3) == 1)
        {
            string streetLetter = Convert.ToChar(random.Next(65, 91)).ToString();
            string streetFloor = random.Next(1, 5).ToString();
            string streetSide = random.Next(1, 3) == 1 ? "tv" : "th";
            streetCombine = $"{streetNumber}{streetLetter}, {streetFloor}{streetSide}";
        }
        else
        {
            streetCombine = streetNumber;
        }

        string streetLongAddress = $"{streets[indexStreet]} {streetCombine}";

        string[] postalCodes = ["1000", "2000", "3000", "4000", "5000", "6000", "7000", "8000"];
        int indexPostal = random.Next(postalCodes.Length);

        Dictionary<string, string> cities = new()
        {
            ["1000"] = "København",
            ["2000"] = "Frederiksberg",
            ["3000"] = "Helsingør",
            ["4000"] = "Roskilde",
            ["5000"] = "Odense",
            ["6000"] = "Kolding",
            ["7000"] = "Esbjerg",
            ["8000"] = "Aarhus",
            ["9000"] = "Aalborg"
        };

        return [streetLongAddress, postalCodes[indexPostal], cities[postalCodes[indexPostal]]];
    }
}
