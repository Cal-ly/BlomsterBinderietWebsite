namespace HttpWebshopCookie.Utilities;

public static class UserExtensions
{
    private static readonly Dictionary<string, List<string>> roleHierarchy = new Dictionary<string, List<string>>
    {
        { "admin", new List<string> { "admin", "manager", "staff", "assistant", "companyrep", "customer" } },
        { "manager", new List<string> { "manager", "staff", "assistant", "companyrep", "customer" } },
        { "staff", new List<string> { "staff", "assistant", "companyrep", "customer" } },
        { "assistant", new List<string> { "assistant", "companyrep", "customer" } },
        { "companyrep", new List<string> { "companyrep", "customer" } },
        { "customer", new List<string> { "customer" } } // Lowest level; has access only to customer level privileges
    };

    public static bool IsAtLeast(this ClaimsPrincipal user, string role)
    {
        if (roleHierarchy.ContainsKey(role))
        {
            var rolesAtOrAbove = roleHierarchy[role];
            return rolesAtOrAbove.Any(userRole => user.IsInRole(userRole));
        }

        return false;
    }
}
