﻿using Microsoft.AspNetCore.Authorization;
using System;

namespace MongoDbStudio.Infrastructure.Authorization
{
    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Scope { get; }

        public HasScopeRequirement(string scope)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }
    }
}
