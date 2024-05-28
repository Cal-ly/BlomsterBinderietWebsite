#pragma warning disable RCS1102 // Make class static.

namespace HttpWebshopCookie.TestStartup;

public class TestStartup
{
    public static WebApplication Create(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("TestDb"));

        builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddIdentityCore<Customer>()
            .AddRoles<IdentityRole>()
            .AddSignInManager<SignInManager<Customer>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddIdentityCore<Employee>()
            .AddRoles<IdentityRole>()
            .AddSignInManager<SignInManager<Employee>>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddSession(options =>
        {
            options.Cookie.Name = ".HttpWebshopCookie.Session";
            options.IdleTimeout = TimeSpan.FromMinutes(20);
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddRazorPages().AddSessionStateTempDataProvider();


        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("adminAccess", policy => policy.RequireRole("admin"));
            options.AddPolicy("managerAccess", policy => policy.RequireRole("admin", "manager"));
            options.AddPolicy("staffAccess", policy => policy.RequireRole("admin", "manager", "staff"));
            options.AddPolicy("assistantAccess", policy => policy.RequireRole("admin", "manager", "staff", "assistant"));
            options.AddPolicy("companyrepAccess", policy => policy.RequireRole("admin", "manager", "staff", "assistant", "companyrep"));
            options.AddPolicy("customerAccess", policy => policy.RequireRole("admin", "manager", "staff", "assistant", "companyrep", "customer"));
        });

        builder.Services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
            options.Password.RequiredUniqueChars = 1;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = true;
        });
        builder.Services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

            options.LoginPath = "/Identity/Account/Login";
            options.AccessDeniedPath = "/Identity/Account/AccessDenied";
            options.SlidingExpiration = true;
        });
        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        builder.Services.AddScoped<ProductService>();
        builder.Services.AddScoped<OrderService>();
        builder.Services.AddScoped<BasketService>();
        builder.Services.AddScoped<TagService>();

        builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
        builder.Services.Configure<SmtpSettings>(options =>
        {
            options.Server = options.Server ?? throw new InvalidOperationException("SmtpSettings:Server not found.");
            options.Port = options.Port == 0 ? throw new InvalidOperationException("SmtpSettings:Port not found.") : options.Port;
            options.SenderName = options.SenderName ?? throw new InvalidOperationException("SmtpSettings:SenderName not found.");
            options.SenderEmail = options.SenderEmail ?? throw new InvalidOperationException("SmtpSettings:SenderEmail not found.");
            options.Username = options.Username ?? throw new InvalidOperationException("SmtpSettings:Username not found.");
            options.Password = options.Password ?? throw new InvalidOperationException("SmtpSettings:Password not found.");
        });

        builder.Services.AddTransient<IEmailService, EmailService>();
        builder.Services.AddTransient<IEmailSender, IdentityEmailSender>();

        return builder.Build();
    }
}

#pragma warning restore RCS1102 // Make class static.