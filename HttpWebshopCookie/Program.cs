global using HttpWebshopCookie.Config;
global using HttpWebshopCookie.Data;
global using HttpWebshopCookie.Data.IndexTables;
global using HttpWebshopCookie.Data.MockData;
global using HttpWebshopCookie.Interfaces;
global using HttpWebshopCookie.Models;
global using HttpWebshopCookie.Models.Users;
global using HttpWebshopCookie.Services;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>() // Register RoleManager<IdentityRole>
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityCore<Customer>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddIdentityCore<Employee>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddRazorPages();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("manager", policy => policy.RequireRole("manager"));
    options.AddPolicy("staff", policy => policy.RequireRole("staff"));
    options.AddPolicy("assistant", policy => policy.RequireRole("assistant"));
    options.AddPolicy("companyrep", policy => policy.RequireRole("companyrep"));
    options.AddPolicy("customer", policy => policy.RequireRole("customer"));
});
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".HttpWebshopCookie.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.IsEssential = true;
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

    //// Lockout settings.
    //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    //options.Lockout.MaxFailedAccessAttempts = 5;
    //options.Lockout.AllowedForNewUsers = true;

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
}

app.UseHttpsRedirection();
app.UseSession();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    SeedRoles.SeedTheRoles(services);
    SeedUsers seedUsers = new(services);
    seedUsers.SeedEmployee();
    seedUsers.SeedCompanies();
    seedUsers.SeedCustomers();
}

app.Run();