global using HttpWebshopCookie.Config;
global using HttpWebshopCookie.Data;
global using HttpWebshopCookie.Data.MockData;
global using HttpWebshopCookie.Interfaces;
global using HttpWebshopCookie.Models;
global using HttpWebshopCookie.Models.Users;
global using HttpWebshopCookie.Services;
global using HttpWebshopCookie.Utilities;
global using HttpWebshopCookie.ViewModels;
global using MimeKit;
global using MailKit.Net.Smtp;
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
global using Microsoft.Extensions.Options;
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
using System.Configuration;

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
    options.AddPolicy("adminAccess", policy => policy.RequireRole("admin"));

    // Manager can do what staff, assistants, company reps, and customers can
    options.AddPolicy("managerAccess", policy => policy.RequireRole("admin", "manager"));

    // Staff can do what assistants, company reps, and customers can
    options.AddPolicy("staffAccess", policy => policy.RequireRole("admin", "manager", "staff"));

    // Assistant can do what company reps and customers can
    options.AddPolicy("assistantAccess", policy => policy.RequireRole("admin", "manager", "staff", "assistant"));

    // Company rep can do what customers can
    options.AddPolicy("companyrepAccess", policy => policy.RequireRole("admin", "manager", "staff", "assistant", "companyrep"));

    // Customer has only customer privileges
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
    SeedRole.SeedRoles(services);
    SeedAllData seedData = new(services);
    await seedData.SeedEmployeeAsync();
    await seedData.SeedCompaniesAsync();
    await seedData.SeedCustomersAsync();
    await seedData.SeedTestCustomerAsync();
    await seedData.SeedGuestsAsync();
    await seedData.SeedProductsAsync();
    await seedData.SeedOrdersAsync();
    await seedData.SeedCompanyOrdersAsync();
    await seedData.SeedBasketActivityAsync();
}

app.Run();