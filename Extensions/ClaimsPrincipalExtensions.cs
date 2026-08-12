using System.Security.Claims;

namespace TaskMngBack.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static List<string> GetPermissions(this ClaimsPrincipal user)
        {
            return user.Claims
                .Where(c => c.Type == "permission")
                .Select(c => c.Value)
                .ToList();
        }
    }
}
