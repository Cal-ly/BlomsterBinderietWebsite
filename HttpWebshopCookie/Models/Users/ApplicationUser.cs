namespace HttpWebshopCookie.Models.Users;

public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public DateTime? EnrollmentDate { get; set; }
    public DateTime? LastLogin { get; set; }
    public virtual Address? Address { get; set; }
    public string? AddressId { get; set; }
    public virtual ICollection<Order> Orders { get; set; } = [];
}

// IdentityUser is a class from the Microsoft.AspNetCore.Identity.EntityFrameworkCore namespace.
// It is a class that represents a user in the identity system.
// it has the following properties:
// - Id: Gets or sets the primary key for this user.
// - UserName: Gets or sets the user name for this user.
// - NormalizedUserName: Gets or sets the normalized user name for this user.
// - Email: Gets or sets the email address for this user.
// - NormalizedEmail: Gets or sets the normalized email address for this user.
// - EmailConfirmed: Gets or sets a flag indicating if the email address has been confirmed.
// - PasswordHash: Gets or sets the password hash for this user.
// - SecurityStamp: Gets or sets a random value that should change whenever a user's credentials have changed (password changed, login removed).
// - ConcurrencyStamp: A random value that should change whenever a user is persisted to the store.
// - PhoneNumber: Gets or sets the telephone number for this user.
// - PhoneNumberConfirmed: Gets or sets a flag indicating if the telephone number has been confirmed.
// - TwoFactorEnabled: Gets or sets a flag indicating if two-factor authentication is enabled for this user.
// - LockoutEnd: Gets or sets the DateTimeOffset for the end of a user's lockout, any time in the past is considered not locked out.
// - LockoutEnabled: Gets or sets a flag indicating if lockout is enabled for this user.
// - AccessFailedCount: Gets or sets the number of failed login attempts for the current user.
// - Claims: Gets the claims for this user.
// - Logins: Gets the external logins for this user.
// - Tokens: Gets the authentication tokens for this user.
// - UserRoles: Gets the roles for this user.
// - UserClaims: Gets the claims for this user.
// - UserLogins: Gets the external logins for this user.
// - UserTokens: Gets the authentication tokens for this user.
