using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MongoDbStudio.Infrastructure.Extensions
{
    public static class PathStringExtensions
    {
        public static string GroupRequestPath(this PathString path)
        {
            var result = Regex.Match(path, @"\/([a-zA-Z0-9])+\/([a-zA-Z0-9])+");

            if (result?.Groups == null || result.Groups.Count == 0 || string.IsNullOrWhiteSpace(result.Groups[0]?.Value))
                return path;
            else
                return result.Groups[0].Value;
        }
    }
}
