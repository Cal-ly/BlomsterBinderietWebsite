namespace HttpWebshopCookie.Data.MockData;

public class SeedAllData(IServiceProvider serviceProvider)
{
    private readonly ApplicationDbContext context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    private readonly UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    private static readonly Random random = new();
    private const string Password = "Tester";

    public static List<string>? EmployeeIdList = [];
    public static List<string>? CompanyIdList = [];
    public static List<string>? CompanyRepIdList = [];
    public static List<Customer> CompanyRepList = [];
    public static List<string>? CustomerIdList = [];
    public static List<string>? GuestIdList = [];
    public static List<string>? ProductIdList = [];
    public static List<Product>? ProductList = [];
    public static List<string>? TagIdList = [];

    public async Task SeedEmployeeAsync()
    {
        List<Address> employeeAddressList = new List<Address>();
        List<Employee> employeeList = new List<Employee>();

        string[] EmployeeRoles = ["admin", "manager", "staff", "assistant"];
        for (int i = 0; i < EmployeeRoles.Length; i++)
        {
            string[] randomAddress = GenerateRandomAddress();
            string[] randomName = GenerateRandomName();
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
        await context.Addresses.AddRangeAsync(employeeAddressList);
        await context.SaveChangesAsync();
        foreach (var employee in employeeList)
        {
            await userManager.CreateAsync(employee, Password);
            await userManager.AddToRoleAsync(employee, employee.JobTitle!);
        }
    }

    public async Task SeedCompaniesAsync()
    {
        List<Address> companyAddresses = [];
        List<Company> companies = [];
        List<Customer> companyReps = [];
        for (int i = 0; i < 6; i++)
        {
            string randomCVR = random.Next(10000000, 99999999).ToString();
            string randomPhone = random.Next(10000000, 99999999).ToString();
            string[] randomAddress = GenerateRandomAddress();
            string[] randomName = GenerateRandomName();
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
            CompanyIdList?.Add(company.Id);

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
            CompanyRepList.Add(companyRep);
        }
        await context.Addresses.AddRangeAsync(companyAddresses);
        await context.Companies.AddRangeAsync(companies);
        await context.SaveChangesAsync();
        foreach (var companyRep in companyReps)
        {
            await userManager.CreateAsync(companyRep, Password);
            await userManager.AddToRoleAsync(companyRep, "companyrep");
        }
    }

    public async Task SeedCustomersAsync()
    {
        List<Address> customerAddresses = [];
        List<Customer> customers = [];
        for (int i = 0; i < 10; i++)
        {
            string[] randomAddress = GenerateRandomAddress();
            string[] randomName = GenerateRandomName();
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
                AddressId = customerAddress.Id,
                Address = customerAddress,
                Title = "Mr/Mrs/Ms",
                BirthDate = DateTime.UtcNow.AddYears(-random.Next(18, 70))
            };
            customers.Add(customer);
            CustomerIdList?.Add(customer.Id);
        }

        await context.Addresses.AddRangeAsync(customerAddresses);
        await context.SaveChangesAsync();
        foreach (var customer in customers)
        {
            await userManager.CreateAsync(customer, Password);
            await userManager.AddToRoleAsync(customer, "customer");
        }
    }

    public async Task SeedTestCustomerAsync()
    {
        string uniqueEmail = "test@test.com";
        string[] randomName = ["Test", "Tester"];
        string[] randomAddress = GenerateRandomAddress();
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

        await context.Addresses.AddAsync(customerAddress);
        await context.SaveChangesAsync();
        await userManager.CreateAsync(customer, Password);
        await userManager.AddToRoleAsync(customer, "customer");
    }

    public async Task SeedGuestsAsync()
    {
        List<Address> guestAddresses = [];
        List<Guest> guests = [];
        for (int i = 0; i < 10; i++)
        {
            string[] randomAddress = GenerateRandomAddress();
            string[] randomName = GenerateRandomName();
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

        await context.Addresses.AddRangeAsync(guestAddresses);
        await context.GuestUsers.AddRangeAsync(guests);
        await context.SaveChangesAsync();
    }

    public async Task SeedTagsAsync()
    {
        TagIdList = [];
        List<Tag> tags = new();
        Dictionary<string, List<(string Category, List<string> SubCategories)>> categoriesByOccasion = new Dictionary<string, List<(string, List<string>)>>
        {
            ["Begravelse"] = new List<(string, List<string>)>
            {
                ("Bårebukket", new List<string> { "Traditionel", "Moderne", "Fredfyldt", "Naturlig" }), // index 0, 1, 2, 3
                ("Kondolencebuket", new List<string> { "Lille", "Mellem", "Stor", "Pastel", "Farverig" }), // index 4, 5, 6, 7, 8
                ("Begravelsesdekoration", new List<string> { "Traditionel", "Moderne", "Monokrom" }), // index 9, 10, 11
                ("Bisættelse", new List<string> { "Traditionel", "Moderne", "Kristen", "Sekulær" }), // index 12, 13, 14, 15
                ("Gravdekoration", new List<string> { "Traditionel", "Moderne", "Elegant", "Farverig", "Pastel" }), // index 16, 17, 18, 19, 20
                ("Blomsterkranse", new List<string> { "Traditionel", "Moderne", "Elegant", "Farverig", "Pastel" }), // index 21, 22, 23, 24, 25
                ("Kistepynt", new List<string> { "Traditionel", "Moderne", "Roser", "Liljer" }), // index 26, 27, 28, 29
                ("Urnepynt", new List<string> { "Traditionel", "Moderne", "Bæredygtig" }) // index 30, 31, 32
            },
            ["Bryllup"] = new List<(string, List<string>)>
            {
                ("Brudebuket", new List<string> {"Klassisk", "Romantisk", "Boheme", "Vintage"}), // index 33, 34, 35, 36
                ("Brudepigebuket", new List<string> {"Simpel", "Moderne", "Farverig", "Minimalistisk"}), // index 37, 38, 39, 40
                ("Bryllupsdekoration", new List<string> {"Traditionel", "Moderne", "Elegant", "Farverig", "Pastel"}), // index 41, 42, 43, 44, 45
                ("Bryllupsgave", new List<string> {"Traditionel", "Moderne", "Elegant", "Farverig", "Pastel"}), // index 46, 47, 48, 49, 50
                ("Bryllupsdag", new List<string> {"Traditionel", "Moderne"}) // index 51, 52

            },
            ["Konfirmation"] = new List<(string, List<string>)>
            {
                ("Konfirmationsbuket", new List<string> {"Sporty", "Musikalsk", "Natur", "Teknologi"}), // index 53, 54, 55, 56
                ("Konfirmationsgave", new List<string> {"Praktisk", "Sjov", "Uddannelsesrig", "Kreativ"}), // index 57, 58, 59, 60
                ("Konfirmationspynt", new List<string> {"Traditionel", "Moderne"}) // index 61, 62
            },
            ["Særlige dage"] = new List<(string, List<string>)>
            {
                ("Fødselsdag", new List<string> {"Ham", "Hende", "Barn"}), // index 63, 64, 65
                ("Nyfødt", new List<string> {"Dreng", "Pige", "Tvillinger"}), // index 66, 67, 68
                ("Årsdag", new List<string> {"Kæreste", "Ægtefælle"}), // index 69, 70
                ("Valentinsdag", new List<string> {"Romantisk", "Sød", "Passioneret", "Elegant"}), // index 71, 72, 73, 74
                ("Mors dag", new List<string> {"Klassisk Mor", "Moderne Mor", "Haveelsker", "Kunstelsker"}), // index 75, 76, 77, 78
                ("Fars dag", new List<string> {"Sporty Far", "Forretningsmand", "Outdoor Far", "Hobbykok"}) // index 79, 80, 81, 82
            },
            ["Kærlig tanke"] = new List<(string, List<string>)>
            {
                ("Tænker på dig", new List<string> {"Lille", "Mellem", "Stor"}), // index 83, 84, 85
                ("God bedring", new List<string> {"Lille", "Mellem", "Stor"}), // index 86, 87, 88
                ("Opmuntring", new List<string> {"Støttende", "Håbefuld", "Optimistisk", "Beroligende"}), // index 89, 90, 91, 92
                ("Held og lykke", new List<string> {"Lille", "Mellem", "Stor"}), // index 93, 94, 95
                ("Tak for", new List<string> {"Lille", "Mellem", "Stor"}), // index 96, 97, 98
                ("Undskyld", new List<string> {"Lille", "Mellem", "Stor"}), // index 99, 100, 101
                ("Romantik", new List<string> {"Passioneret", "Sød", "Intim", "Grand Amor"}) // index 102, 103, 104, 105
            },
            ["Gave"] = new List<(string, List<string>)>
            {
                ("Barselsgaver", new List<string> {"Lille", "Mellem", "Stor"}), // index 106, 107, 108
                ("Gaveideer til hende", new List<string> {"Fashionista", "Fitness", "Bogelsker"}), // index 109, 110, 111
                ("Gaveideer til ham", new List<string> {"Teknologi", "Sport", "Musik", "Handyman"}), // index 112, 113, 114, 115
                ("Indflyttergave", new List<string> {"Lille", "Mellem", "Stor"}), // index 116, 117, 118
                ("Værtindegave", new List<string> {"Lille", "Mellem", "Stor"}) // index 119, 120, 121
            },
            ["Hjemmet"] = new List<(string, List<string>)>
            {
                ("Gulvplante", new List<string> {"Lille", "Mellem", "Stor", "Lav vedligeholdelse", "Luftrensende", "Skygge-tolerant", "Farverig"}), // index 122, 123, 124, 125, 126, 127, 128
                ("Stueplante", new List<string> {"Lille", "Mellem", "Stor", "Lav vedligeholdelse", "Luftrensende", "Skygge-tolerant", "Farverig"}), // index 129, 130, 131, 132, 133, 134, 135
                ("Udendørsplante", new List<string> {"Lille", "Mellem", "Stor"}) // index 136, 137, 138
            },
            ["Arbejde"] = new List<(string, List<string>)>
            {
                ("Første arbejdsdag", new List<string> {"Lille", "Mellem", "Stor"}), // index 139, 140, 141
                ("Fødselsdag", new List<string> {"Lille", "Mellem", "Stor"}), // index 142, 143, 144
                ("Jubilæum", new List<string> {"Lille", "Mellem", "Stor"}), // index 145, 146, 147
                ("Pension", new List<string> {"Lille", "Mellem", "Stor"}) // index 148, 149, 150
            },
            ["Sæson"] = new List<(string, List<string>)>
            {
                ("Forår", new List<string> {"Lille", "Mellem", "Stor"}), // index 151, 152, 153
                ("Sommer", new List<string> {"Lille", "Mellem", "Stor"}), // index 154, 155, 156
                ("Efterår", new List<string> {"Lille", "Mellem", "Stor"}), // index 157, 158, 159
                ("Vinter", new List<string> {"Lille", "Mellem", "Stor"}) // index 160, 161, 162
            },
            ["Blomster"] = new List<(string, List<string>)>
            {
                ("Roser", new List<string> {"Røde", "Gule", "Hvide"}), // index 163, 164, 165
                ("Liljer", new List<string> {"Hvide", "Gule", "Rosa"}), // index 166, 167, 168
                ("Orkideer", new List<string> {"Hvide", "Rosa", "Lilla"}), // index 169, 170, 171
                ("Tulipaner", new List<string> {"Røde", "Gule", "Hvide"}), // index 172, 173, 174
                ("Gerbera", new List<string> {"Røde", "Gule", "Hvide"}), // index 175, 176, 177
                ("Solsikker", new List<string> {"Store", "Små", "Mellem"}), // index 178, 179, 180
                ("Hortensia", new List<string> {"Blå", "Rosa", "Hvide"}), // index 181, 182, 183
                ("Valmuer", new List<string> {"Røde", "Gule", "Hvide"}), // index 184, 185, 186
                ("Krysantemum", new List<string> {"Røde", "Gule", "Hvide"}), // index 187, 188, 189
                ("Peon", new List<string> {"Røde", "Gule", "Hvide"}), // index 190, 191, 192
                ("Anemone", new List<string> {"Røde", "Gule", "Hvide"}), // index 193, 194, 195
                ("Amaryllis", new List<string> {"Røde", "Gule", "Hvide"}), // index 196, 197, 198
                ("Freesia", new List<string> {"Røde", "Gule", "Hvide"}), // index 199, 200, 201
                ("Lavendel", new List<string> {"Blå", "Rosa", "Hvide"}), // index 202, 203, 204
                ("Kornblomst", new List<string> {"Blå", "Rosa", "Hvide"}) // index 205, 206
            }
        };
        foreach (var occasion in categoriesByOccasion)
        {
            foreach (var category in occasion.Value)
            {
                foreach (var subCategory in category.SubCategories)
                {
                    var tag = new Tag
                    {
                        Occasion = occasion.Key,
                        Category = category.Category,
                        SubCategory = subCategory
                    };
                    tags.Add(tag);
                    TagIdList.Add(tag.Id);
                }
            }
        }
        await context.Tags.AddRangeAsync(tags);
    }

    public async Task SeedProductsAsync()
    {
        await SeedTagsAsync();
        ProductIdList = [];
        ProductList = new List<Product>
        {
            new Product
            {
                Name = "Ærefuld Farvel",
                Description = "En dybfølt og respektfuld buket til at ære en elsket. Traditionelle hvide liljer og roser.",
                Price = 350,
                ImageUrl = "\\images\\products\\honorable-goodbye.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Pastel Kondolence",
                Description = "Beroligende pastelfarvede blomster til at udtrykke sympati.",
                Price = 300,
                ImageUrl = "\\images\\products\\pastel-condolence.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Sekulær Bisættelse",
                Description = "Diskret og enkel buket til en sekulær afskedsceremoni.",
                Price = 350,
                ImageUrl = "\\images\\products\\secular-burial.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Romantisk Gestus",
                Description = "En passioneret rød rosenbuket, der udtrykker dyb kærlighed og hengivenhed.",
                Price = 450,
                ImageUrl = "\\images\\products\\romantic-gesture.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Moderne Elegance",
                Description = "En stilfuld og moderne arrangement med orkideer og eksotiske blomster.",
                Price = 500,
                ImageUrl = "\\images\\products\\modern-elegance.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Fødselsdagsglæde",
                Description = "Farverig og festlig buket der lyser op i enhver fødselsdagsfest.",
                Price = 300,
                ImageUrl = "\\images\\products\\birthday-joy.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Nyfødte Lykønskninger",
                Description = "Sød og blid buket til at fejre ankomsten af et nyt barn.",
                Price = 350,
                ImageUrl = "\\images\\products\\newborn-celebrations.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Stilfuld Forretningsgave",
                Description = "Eksklusiv plante der tilføjer en professionel atmosfære til ethvert kontor.",
                Price = 600,
                ImageUrl = "\\images\\products\\stylish-businessgift.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Til Konfirmanten",
                Description = "Friske og unge toner i en buket, der passer perfekt til en stor dag.",
                Price = 250,
                ImageUrl = "\\images\\products\\to-the-konfirmant.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Efterårsharmoni",
                Description = "Varm og indbydende buket der afspejler efterårets farver.",
                Price = 375,
                ImageUrl = "\\images\\products\\autumn-harmony.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Vintermagi",
                Description = "En kold og krystalklar buket, der bringer vinterens essens indendørs.",
                Price = 400,
                ImageUrl = "\\images\\products\\wintermagic.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Livlige Sommerblomster",
                Description = "En sprudlende og energisk buket der fanger sommerens ånd.",
                Price = 325,
                ImageUrl = "\\images\\products\\lively-summerflowers.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Boheme Brudebuket",
                Description = "Unik og kunstnerisk buket designet til den moderne brud.",
                Price = 800,
                ImageUrl = "\\images\\products\\boheme-bridebouquet.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Klassisk Mors Dags Buket",
                Description = "Traditionel og tidløs buket for at hylde alle mødre.",
                Price = 275,
                ImageUrl = "\\images\\products\\classical-mothersday.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            },
            new Product
            {
                Name = "Haveelskerens Favorit",
                Description = "Diverse og naturlige blomster valgt til en ægte haveelsker.",
                Price = 400,
                ImageUrl = "\\images\\products\\gardenlovers-favorite.png",
                IsDeleted = false,
                UpdatedAt = DateTime.Now
            }
        };

        ProductIdList.AddRange(ProductList.Select(p => p.Id));
        await context.Products.AddRangeAsync(ProductList);

        if (TagIdList is null)
            return;

        var productTags = new List<IXProductTag>
        {
            new IXProductTag { ProductId = ProductIdList[0], TagId = TagIdList[0] },
            new IXProductTag { ProductId = ProductIdList[0], TagId = TagIdList[165] },
            new IXProductTag { ProductId = ProductIdList[0], TagId = TagIdList[166] },
            new IXProductTag { ProductId = ProductIdList[1], TagId = TagIdList[4] },
            new IXProductTag { ProductId = ProductIdList[1], TagId = TagIdList[7] },
            new IXProductTag { ProductId = ProductIdList[2], TagId = TagIdList[15] },
            new IXProductTag { ProductId = ProductIdList[3], TagId = TagIdList[102] },
            new IXProductTag { ProductId = ProductIdList[3], TagId = TagIdList[163] },
            new IXProductTag { ProductId = ProductIdList[4], TagId = TagIdList[124] },
            new IXProductTag { ProductId = ProductIdList[4], TagId = TagIdList[128] },
            new IXProductTag { ProductId = ProductIdList[4], TagId = TagIdList[169] },
            new IXProductTag { ProductId = ProductIdList[5], TagId = TagIdList[63] },
            new IXProductTag { ProductId = ProductIdList[6], TagId = TagIdList[66] },
            new IXProductTag { ProductId = ProductIdList[7], TagId = TagIdList[114] },
            new IXProductTag { ProductId = ProductIdList[8], TagId = TagIdList[55] },
            new IXProductTag { ProductId = ProductIdList[9], TagId = TagIdList[158] },
            new IXProductTag { ProductId = ProductIdList[10], TagId = TagIdList[161] },
            new IXProductTag { ProductId = ProductIdList[11], TagId = TagIdList[155] },
            new IXProductTag { ProductId = ProductIdList[12], TagId = TagIdList[36] },
            new IXProductTag { ProductId = ProductIdList[13], TagId = TagIdList[75] },
            new IXProductTag { ProductId = ProductIdList[13], TagId = TagIdList[77] },
            new IXProductTag { ProductId = ProductIdList[14], TagId = TagIdList[126] }
        };

        await context.ProductTags.AddRangeAsync(productTags);
    }

    public async Task SeedOrdersAsync()
    {
        List<Order> orders = new List<Order>();
        for (int i = 0; i < 25; i++)
        {
            var randomDates = GenerateRandomDates();
            var randomProduct1 = random.Next(0, ProductList!.Count);
            var randomProduct2 = random.Next(0, ProductList!.Count - 1);
            if (randomProduct2 >= randomProduct1)
            {
                randomProduct2++;
            }
            List<OrderItem> orderItemList = new List<OrderItem>
            {
                new OrderItem { ProductId = ProductList![randomProduct1].Id, Quantity = random.Next(1, 2), UnitPrice = ProductList![randomProduct1].Price },
                new OrderItem { ProductId = ProductList![randomProduct2].Id, Quantity = random.Next(1, 2), UnitPrice = ProductList![randomProduct2].Price }
            };
            var order = new Order
            {
                CustomerId = CustomerIdList![random.Next(0, CustomerIdList.Count)],
                OrderDate = DateTime.Now.AddDays(randomDates[0]),
                CompletionDate = DateTime.Now.AddDays(randomDates[1]),
                Status = OrderStatus.Completed,
                OrderItems = new List<OrderItem>(),
            };
            order.OrderItems.AddRange(orderItemList);
            orders.Add(order);
        }
        for (int i = 0; i < 25; i++)
        {
            var randomDates = GenerateRandomDates();
            var randomProduct1 = random.Next(0, ProductList!.Count);
            var randomProduct2 = random.Next(0, ProductList!.Count - 1);
            if (randomProduct2 >= randomProduct1)
            {
                randomProduct2++;
            }
            List<OrderItem> orderItemList = new List<OrderItem>
            {
                new OrderItem { ProductId = ProductList![randomProduct1].Id, Quantity = random.Next(1, 2), UnitPrice = ProductList![randomProduct1].Price },
                new OrderItem { ProductId = ProductList![randomProduct2].Id, Quantity = random.Next(1, 2), UnitPrice = ProductList![randomProduct2].Price }
            };
            var order = new Order
            {
                GuestId = GuestIdList![random.Next(0, GuestIdList.Count)],
                OrderDate = DateTime.Now.AddDays(randomDates[0]),
                CompletionDate = DateTime.Now.AddDays(randomDates[1]),
                Status = OrderStatus.Completed,
                OrderItems = new List<OrderItem>(),
            };
            order.OrderItems.AddRange(orderItemList);
            orders.Add(order);
        }
        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }
    public async Task SeedCompanyOrdersAsync()
    {
        List<Order> orders = new List<Order>();
        List<SpecialOrderInstruction> soiList = new List<SpecialOrderInstruction>();
        for (int i = 0; i < CompanyRepList?.Count; i++)
        {
            var randomDates = GenerateRandomDates();
            var randomProduct1 = random.Next(0, ProductList!.Count);
            var randomProduct2 = random.Next(0, ProductList!.Count - 1);

            if (randomProduct2 >= randomProduct1)
            {
                randomProduct2++;
            }

            List<OrderItem> orderItemList = new List<OrderItem>
            {
                new OrderItem { ProductId = ProductList![randomProduct1].Id, Quantity = random.Next(1, 2), UnitPrice = ProductList![randomProduct1].Price },
                new OrderItem { ProductId = ProductList![randomProduct2].Id, Quantity = random.Next(1, 2), UnitPrice = ProductList![randomProduct2].Price }
            };

            var order = new Order
            {
                Customer = CompanyRepList![i],
                CustomerId = CompanyRepList![i].Id,
                OrderDate = DateTime.Now.AddDays(randomDates[0]),
                CompletionDate = DateTime.Now.AddDays(randomDates[1]),
                Status = OrderStatus.Completed,
                OrderItems = new List<OrderItem>(),
            };
            order.OrderItems.AddRange(orderItemList);

            var soi = new SpecialOrderInstruction
            {
                OrderId = order.Id,
                Order = order,
                SpecialInstructions = "Opstilling i kappel, Fredag 24/05 kl 1000",
                Delivery = true,
                Arrangement = true,
                SpecialDeliveryAddressId = order.Customer.AddressId,
                SpecialDeliveryAddress = order.Customer.Address
            };

            order.SpecialOrderInstruction = soi;
            order.SpecialOrderInstructionsId = soi.Id;
            soiList.Add(soi);
            orders.Add(order);
        }
        await context.SpecialOrderInstructions.AddRangeAsync(soiList);
        await context.Orders.AddRangeAsync(orders);
        await context.SaveChangesAsync();
    }

    public async Task SeedBasketActivityAsync()
    {
        List<BasketActivity> basketActivities = new();

        for (int i = 0; i < 50; i++)
        {
            var userId = CustomerIdList![random.Next(CustomerIdList.Count)];
            var isRegisteredUser = userId != null;
            DateTime dateTime = DateTime.UtcNow.AddDays(random.Next(-365, 1));
            for (int j = 0; j < random.Next(4, 10); j++)
            {
                var productId = ProductIdList![random.Next(ProductIdList.Count)];
                var quantityChanged = random.Next(1, 2) * (random.Next(2) == 0 ? 1 : -1);
                var activityType = quantityChanged > 0 ? "Add" : "Remove";
                BasketActivity activity = new()
                {
                    ActivityType = activityType,
                    QuantityChanged = quantityChanged,
                    UserId = userId,
                    IsRegisteredUser = isRegisteredUser,
                    ProductId = productId,
                    Timestamp = dateTime.AddSeconds(random.Next(5, 20))
                };
                basketActivities.Add(activity);
            }
        }
        for (int i = 0; i < 50; i++)
        {
            var sessionId = Guid.NewGuid().ToString();
            DateTime dateTime = DateTime.UtcNow.AddDays(random.Next(-365, 1));
            for (int j = 0; j < random.Next(4, 10); j++)
            {
                var productId = ProductIdList![random.Next(ProductIdList.Count)];
                var quantityChanged = random.Next(1, 2) * (random.Next(2) == 0 ? 1 : -1);
                var activityType = quantityChanged > 0 ? "Add" : "Remove";
                BasketActivity activity = new()
                {
                    ActivityType = activityType,
                    QuantityChanged = quantityChanged,
                    SessionId = sessionId,
                    ProductId = productId,
                    Timestamp = dateTime.AddSeconds(random.Next(5, 20))
                };
                basketActivities.Add(activity);
            }
        }
        await context.BasketActivities.AddRangeAsync(basketActivities);
        await context.SaveChangesAsync();
    }

    public static string[] GenerateRandomName()
    {
        string[] firstNames = ["Hans", "Jens", "Lone", "John", "Emma", "Michael", "Sophia", "William", "Olivia", "Kristian"];
        int indexFirst = random.Next(firstNames.Length);

        string[] lastNames = ["Hansen", "Jensen", "Johnson", "Gormsen", "Michaelson", "Kristiansdottir", "Wilson", "Olivera"];
        int indexLast = random.Next(lastNames.Length);

        return [firstNames[indexFirst], lastNames[indexLast]];
    }

    public static string[] GenerateRandomAddress()
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

    public static int[] GenerateRandomDates()
    {
        int orderDate = random.Next(-365, -4);
        int completionDate = orderDate + random.Next(1, 3);
        return [orderDate, completionDate];
    }
}