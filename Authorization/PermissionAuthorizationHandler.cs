using Microsoft.AspNetCore.Authorization;

namespace TaskMngBack.Authorization
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User.HasClaim("permission", requirement.PermissionKey))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
