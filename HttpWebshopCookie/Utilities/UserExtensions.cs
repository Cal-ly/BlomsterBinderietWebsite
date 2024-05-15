namespace HttpWebshopCookie.Utilities;

public static class UserExtensions
{
    private static readonly Dictionary<string, HashSet<string>> roleHierarchy = new()
    {
        { "admin", new HashSet<string> { "admin", "manager", "staff", "assistant", "companyrep", "customer" } },
        { "manager", new HashSet<string> { "manager", "staff", "assistant", "companyrep", "customer" } },
        { "staff", new HashSet<string> { "staff", "assistant", "companyrep", "customer" } },
        { "assistant", new HashSet<string> { "assistant", "companyrep", "customer" } },
        { "companyrep", new HashSet<string> { "companyrep", "customer" } },
        { "customer", new HashSet<string> { "customer" } }
    };

    public static bool IsAtLeast(this ClaimsPrincipal user, string role)
    {
        if (roleHierarchy.TryGetValue(role, out HashSet<string>? value))
        {
            return value.Any(user.IsInRole);
        }

        return false;
    }
}