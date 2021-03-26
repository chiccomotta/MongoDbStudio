using Microsoft.AspNetCore.Authorization;
using System;

namespace MongoDbStudio.Infrastructure.Authorization
{
    public class HasCustomClaimRequirement : IAuthorizationRequirement
    {
        public string CustomClaimName { get; }
        public string CustomClaimValue { get; }

        public HasCustomClaimRequirement(string customClaimName, string customClaimValue)
        {
            CustomClaimName = customClaimName ?? throw new ArgumentNullException(nameof(customClaimName));
            CustomClaimValue = customClaimValue ?? throw new ArgumentNullException(nameof(customClaimValue));
        }
    }
}
