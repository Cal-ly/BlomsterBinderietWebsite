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

    //TODO: Add seed methods for other entities

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
    }
}
