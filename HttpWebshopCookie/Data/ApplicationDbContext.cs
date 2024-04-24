namespace HttpWebshopCookie.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<GuestUser> GuestUsers { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
    public DbSet<BasketActivity> BasketActivities { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ProductTag> ProductTags { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply other configurations...

        base.OnModelCreating(modelBuilder);
        SeedRoles(modelBuilder);
        SeedUsers(modelBuilder);
        SeedProducts(modelBuilder);

        modelBuilder.ApplyConfiguration(new CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new EmployeeConfiguration());
        modelBuilder.ApplyConfiguration(new GuestConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new CompanyConfiguration());
        modelBuilder.ApplyConfiguration(new ProductConfiguration());
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new BasketConfiguration());
        modelBuilder.ApplyConfiguration(new BasketItemConfiguration());
        modelBuilder.ApplyConfiguration(new BasketActivityConfiguration());
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new ProductTagConfiguration());
    }

    private static void SeedRoles(ModelBuilder modelBuilder)
    {
        string[] roleNames = ["Admin", "Manager", "Staff", "Assistant", "CompanyRep", "Registered"];
        modelBuilder.Entity<IdentityRole>().HasData(
        new IdentityRole{ Id = "0", Name = roleNames[0], NormalizedName = roleNames[0].ToUpper() },
        new IdentityRole{ Id = "1", Name = roleNames[1], NormalizedName = roleNames[1].ToUpper() },
        new IdentityRole{ Id = "2", Name = roleNames[2], NormalizedName = roleNames[2].ToUpper() },
        new IdentityRole{ Id = "3", Name = roleNames[3], NormalizedName = roleNames[3].ToUpper() },
        new IdentityRole{ Id = "4", Name = roleNames[4], NormalizedName = roleNames[4].ToUpper() },
        new IdentityRole{ Id = "5", Name = roleNames[5], NormalizedName = roleNames[5].ToUpper() });
    }
    private void SeedUsers(ModelBuilder modelBuilder)
    {
        var hasher = new PasswordHasher<ApplicationUser>();
        string password = "Test.1234";
        string[] userNames = ["Admin", "Manager", "Staff", "Assistant", "CompanyRep", "Registered"];
        string[] emails = ["admin@shop.com", "manager@shop.com", "staff@shop.com", "assistant@shop.com", "companyrep@shop.com", "registred@shop.com"];

        Employee adminUser = new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userNames[0],
            NormalizedUserName = userNames[0].ToUpper(),
            Email = emails[0],
            NormalizedEmail = emails[0].ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null!, password), // replace with your initial password
            SecurityStamp = string.Empty,
            JobTitle = "CEO",
            Salary = 100000,
            HireDate = DateTime.Now
        };
        Employee managerUser = new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userNames[1],
            NormalizedUserName = userNames[1].ToUpper(),
            Email = emails[1],
            NormalizedEmail = emails[1].ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null!, password), // replace with your initial password
            SecurityStamp = string.Empty,
            JobTitle = "Manager",
            Salary = 50000,
            HireDate = DateTime.Now
        };
        Employee staffUser = new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userNames[2],
            NormalizedUserName = userNames[2].ToUpper(),
            Email = emails[2],
            NormalizedEmail = emails[2].ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null!, password), // replace with your initial password
            SecurityStamp = string.Empty,
            JobTitle = "Staff",
            Salary = 30000,
            HireDate = DateTime.Now
        };
        Employee assistantUser = new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userNames[3],
            NormalizedUserName = userNames[3].ToUpper(),
            Email = emails[3],
            NormalizedEmail = emails[3].ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null!, password), // replace with your initial password
            SecurityStamp = string.Empty,
            JobTitle = "Assistant",
            Salary = 20000,
            HireDate = DateTime.Now
        };
        Customer companyRepUser = new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userNames[4],
            NormalizedUserName = userNames[4].ToUpper(),
            Email = emails[4],
            NormalizedEmail = emails[4].ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null!, password), // replace with your initial password
            SecurityStamp = string.Empty,
            Title = "Mr",
            FirstName = "Doey",
            LastName = "Johnson",
            PhoneNumber = "92345678"
        };
        Customer registeredUser = new()
        {
            Id = Guid.NewGuid().ToString(),
            UserName = userNames[5],
            NormalizedUserName = userNames[5].ToUpper(),
            Email = emails[5],
            NormalizedEmail = emails[5].ToUpper(),
            EmailConfirmed = true,
            PasswordHash = hasher.HashPassword(null!, password), // replace with your initial password
            SecurityStamp = string.Empty,
            Title = "Mr",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "12345678",
            Address = new Address
            {
                Id = Guid.NewGuid().ToString(),
                Street = "Test Street",
                City = "Test City",
                PostalCode = "1234",
                Country = "Test Country"
            },
        };

        modelBuilder.Entity<Employee>().HasData(adminUser, managerUser, staffUser, assistantUser);
        modelBuilder.Entity<Customer>().HasData(companyRepUser, registeredUser);
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = adminUser.Id, RoleId = "0" });
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = managerUser.Id, RoleId = "1" });
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = staffUser.Id, RoleId = "2" });
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = assistantUser.Id, RoleId = "3" });
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = companyRepUser.Id, RoleId = "4" });
        modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string> { UserId = registeredUser.Id, RoleId = "5" });
    }

    public void SeedProducts(ModelBuilder modelBuilder)
    {
        Product product1 = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Product 1",
            Description = "Description of Product 1",
            Price = 100,
            ImageUrl = "https://via.placeholder.com/150",
            IsActivated = true,
            UpdatedAt = DateTime.Now
        };
        Product product2 = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Product 2",
            Description = "Description of Product 2",
            Price = 200,
            ImageUrl = "https://via.placeholder.com/150",
            IsActivated = true,
            UpdatedAt = DateTime.Now
        };
        Product product3 = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Product 3",
            Description = "Description of Product 3",
            Price = 300,
            ImageUrl = "https://via.placeholder.com/150",
            IsActivated = true,
            UpdatedAt = DateTime.Now
        };
        Product product4 = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Product 4",
            Description = "Description of Product 4",
            Price = 400,
            ImageUrl = "https://via.placeholder.com/150",
            IsActivated = true,
            UpdatedAt = DateTime.Now
        };
        Product product5 = new Product
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Product 5",
            Description = "Description of Product 5",
            Price = 500,
            ImageUrl = "https://via.placeholder.com/150",
            IsActivated = true,
            UpdatedAt = DateTime.Now
        };

        modelBuilder.Entity<Product>().HasData(product1, product2, product3, product4, product5);

        Tag tag1 = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Tag 1",
            Catergory = "Category 1",
            SubCategory = "SubCategory 1",

        };
        Tag tag2 = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Tag 2",
            Catergory = "Category 2",
            SubCategory = "SubCategory 2"
        };
        Tag tag3 = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Tag 3",
            Catergory = "Category 3",
            SubCategory = "SubCategory 3"
        };
        Tag tag4 = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Tag 4",
            Catergory = "Category 4",
            SubCategory = "SubCategory 4"
        };
        Tag tag5 = new Tag
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Tag 5",
            Catergory = "Category 5",
            SubCategory = "SubCategory 5"
        };

        modelBuilder.Entity<Tag>().HasData(tag1, tag2, tag3, tag4, tag5);

        modelBuilder.Entity<ProductTag>().HasData(
            new ProductTag { ProductId = product1.Id, TagId = tag1.Id },
            new ProductTag { ProductId = product1.Id, TagId = tag2.Id },
            new ProductTag { ProductId = product2.Id, TagId = tag2.Id },
            new ProductTag { ProductId = product2.Id, TagId = tag3.Id },
            new ProductTag { ProductId = product3.Id, TagId = tag3.Id },
            new ProductTag { ProductId = product3.Id, TagId = tag4.Id },
            new ProductTag { ProductId = product4.Id, TagId = tag4.Id },
            new ProductTag { ProductId = product4.Id, TagId = tag5.Id },
            new ProductTag { ProductId = product5.Id, TagId = tag5.Id },
            new ProductTag { ProductId = product5.Id, TagId = tag1.Id }
        );
    }

    public void SeedAddresses(ModelBuilder modelBuilder)
    {
        Address companyAddress1 = new Address
        {
            Id = "1",
            Resident = "Company 1",
            Street = "Company Street 1",
            City = "Company City 1",
            PostalCode = "1234",
            Country = "Company Country 1"
        };
        Address companyAddress2 = new Address
        {
            Id = "2",
            Resident = "Company 2",
            Street = "Company Street 2",
            City = "Company City 2",
            PostalCode = "5678",
            Country = "Company Country 2"
        };
        Address companyAddress3 = new Address
        {
            Id = "3",
            Resident = "Company 3",
            Street = "Company Street 3",
            City = "Company City 3",
            PostalCode = "9012",
            Country = "Company Country 3"
        };
        Address companyAddress4 = new Address
        {
            Id = "4",
            Resident = "Company 4",
            Street = "Company Street 4",
            City = "Company City 4",
            PostalCode = "3456",
            Country = "Company Country 4"
        };
        Address companyAddress5 = new Address
        {
            Id = "5",
            Resident = "Company 5",
            Street = "Company Street 5",
            City = "Company City 5",
            PostalCode = "7890",
            Country = "Company Country 5"
        };
        Address companyAddress6 = new Address
        {
            Id = "6",
            Resident = "Company 6",
            Street = "Company Street 6",
            City = "Company City 6",
            PostalCode = "1234",
            Country = "Company Country 6"
        };
        Address address1 = new Address
        {
            Id = "11",
            Resident = "John Doe",
            Street = "Test Street 1",
            City = "Test City 1",
            PostalCode = "1234",
            Country = "Test Country 1"
        };
        Address address2 = new Address
        {
            Id = "12",
            Resident = "Jane Doe",
            Street = "Test Street 2",
            City = "Test City 2",
            PostalCode = "5678",
            Country = "Test Country 2"
        };
        Address address3 = new Address
        {
            Id = "13",
            Resident = "Jack Doe",
            Street = "Test Street 3",
            City = "Test City 3",
            PostalCode = "9012",
            Country = "Test Country 3"
        };
        Address address4 = new Address
        {
            Id = "14",
            Resident = "Jill Doe",
            Street = "Test Street 4",
            City = "Test City 4",
            PostalCode = "3456",
            Country = "Test Country 4"
        };
        Address address5 = new Address
        {
            Id = "15",
            Resident = "Jim Doe",
            Street = "Test Street 5",
            City = "Test City 5",
            PostalCode = "7890",
            Country = "Test Country 5"
        };
        Address address6 = new Address
        {
            Id = "16",
            Resident = "Jenny Doe",
            Street = "Test Street 6",
            City = "Test City 6",
            PostalCode = "1234",
            Country = "Test Country 6"
        };


        modelBuilder.Entity<Address>().HasData(companyAddress1, companyAddress2, companyAddress3, companyAddress4, companyAddress5, companyAddress6,
               address1, address2, address3, address4, address5, address6);
    }

    public void SeedCompanies(ModelBuilder modelBuilder)
    {
        Company company1 = new Company
        {
            Id = "1",
            CVR = "12345678",
            Name = "Company 1",
            PhoneNumber = "12345678",
            AddressId = "1"
        };
        Company company2 = new Company
        {
            Id = "2",
            CVR = "23456789",
            Name = "Company 2",
            PhoneNumber = "23456789",
            AddressId = "2"
        };
        Company company3 = new Company
        {
            Id = "3",
            CVR = "34567890",
            Name = "Company 3",
            PhoneNumber = "34567890",
            AddressId = "3"
        };
        Company company4 = new Company
        {
            Id = "4",
            CVR = "45678901",
            Name = "Company 4",
            PhoneNumber = "45678901",
            AddressId = "4"
        };
        Company company5 = new Company
        {
            Id = "5",
            CVR = "56789012",
            Name = "Company 5",
            PhoneNumber = "56789012",
            AddressId = "5"
        };
        Company company6 = new Company
        {
            Id = "6",
            CVR = "67890123",
            Name = "Company 6",
            PhoneNumber = "67890123",
            AddressId = "6"
        };

        modelBuilder.Entity<Company>().HasData(company1, company2, company3, company4, company5, company6);
    }

    public void SeedOrders(ModelBuilder modelBuilder)
    { }

    public void SeedOrderItems(ModelBuilder modelBuilder)
    { }

    public void SeedBaskets(ModelBuilder modelBuilder)
    { }

    public void SeedBasketItems(ModelBuilder modelBuilder)
    { }

    public void SeedBasketActivities(ModelBuilder modelBuilder)
    { }

}
