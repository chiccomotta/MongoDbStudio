using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace MongoDbStudio.Infrastructure.Authorization
{
    public class HasCustomClaimHandler : AuthorizationHandler<HasCustomClaimRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasCustomClaimRequirement requirement)
        {
            // If user does not have the scope claim, get out of here
            if (!context.User.HasClaim(c => c.Type == requirement.CustomClaimName))
                return Task.CompletedTask;

            // Split the scopes string into an array
            var value = context.User.FindFirst(c => c.Type == requirement.CustomClaimName).Value;

            // Succeed if the scope array contains the required scope
            if (value == requirement.CustomClaimValue)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}

