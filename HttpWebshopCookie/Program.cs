global using HttpWebshopCookie.ViewComponents;
global using HttpWebshopCookie.Config;
global using HttpWebshopCookie.Data;
global using HttpWebshopCookie.Data.MockData;
global using HttpWebshopCookie.Models;
global using HttpWebshopCookie.Models.Users;
global using HttpWebshopCookie.Services;
global using HttpWebshopCookie.Utilities;
global using HttpWebshopCookie.ViewModels;
global using Microsoft.AspNetCore.Authentication;
global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Hosting;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Identity;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
global using Microsoft.AspNetCore.Identity.UI.Services;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.AspNetCore.Mvc.RazorPages;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel.DataAnnotations;
global using System.IO;
global using System.Linq;
global using System.Security.Claims;
global using System.Text;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

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
    // Admin can do what managers, staff, assistants, company reps, and customers can
    options.AddPolicy("AdminAccess", policy => policy.RequireRole("admin", "manager", "staff", "assistant", "companyrep", "customer"));

    // Manager can do what staff, assistants, company reps, and customers can
    options.AddPolicy("ManagerAccess", policy => policy.RequireRole("manager", "staff", "assistant", "companyrep", "customer"));

    // Staff can do what assistants, company reps, and customers can
    options.AddPolicy("StaffAccess", policy => policy.RequireRole("staff", "assistant", "companyrep", "customer"));

    // Assistant can do what company reps and customers can
    options.AddPolicy("AssistantAccess", policy => policy.RequireRole("assistant", "companyrep", "customer"));

    // Company rep can do what customers can
    options.AddPolicy("CompanyRepAccess", policy => policy.RequireRole("companyrep", "customer"));

    // Customer has only customer privileges
    options.AddPolicy("CustomerAccess", policy => policy.RequireRole("customer"));
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

//app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
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
    seedUsers.SeedTester();
}

app.Run();