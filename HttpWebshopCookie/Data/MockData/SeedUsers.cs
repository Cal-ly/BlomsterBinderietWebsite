namespace HttpWebshopCookie.Data.MockData;

public class SeedUsers()
{
    private static readonly Random random = new();
    private const string Password = "Tester";

    public static List<string>? EmployeeIdList;
    public static List<string>? CompanyRepIdList;
    public static List<string>? CustomerIdList;
    public static List<string>? GuestIdList;
    public static List<string>? ProductIdList;
    public static List<string>? TagIdList;

    public static void SeedEmployee(IServiceProvider serviceProvider)
    {
        EmployeeIdList = [];
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        List<Address> employeeAddressList = new List<Address>();
        List<Employee> employeeList = new List<Employee>();

        string[] EmployeeRoles = ["Admin", "Manager", "Staff", "Assistant"];
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
                UserName = EmployeeRoles[i],
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
                Salary = 100000,
                EnrollmentDate = DateTime.UtcNow,
                AddressId = employeeAddress.Id
            };
            EmployeeIdList.Add(employeeUser.Id);
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

    public static void SeedCompanies(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        CompanyRepIdList = [];
        List<Address> companyAddresses = [];
        List<Company> companies = [];
        List<Customer> companyReps = [];
        for (int i = 0; i < 6; i++)
        {
            string randomCVR = random.Next(10000000, 99999999).ToString();
            string randomPhone = random.Next(10000000, 99999999).ToString();
            string[] randomAddress = GenerateRandomAddress(random);
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
            string[] randomName = GenerateRandomName(random);
            Customer companyRep = new()
            {
                UserName = $"CompanyRep{i}",
                NormalizedUserName = $"CompanyRep{i}".ToUpper(),
                Email = $"companyrep{i}@rep.com",
                NormalizedEmail = $"companyrep{i}@test.com".ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = string.Empty,
                Title = "Owner",
                FirstName = randomName[0],
                LastName = randomName[1],
                PhoneNumber = randomPhone,
                AddressId = company.AddressId
            };
            CompanyRepIdList.Add(companyRep.Id);
        }
        context.AddRange(companyAddresses);
        context.AddRange(companies);
        context.SaveChanges();
        foreach (var companyRep in companyReps)
        {
            userManager.CreateAsync(companyRep, Password).Wait();
            userManager.AddToRoleAsync(companyRep, "CompanyRep").Wait();
        }
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
