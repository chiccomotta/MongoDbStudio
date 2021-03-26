using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using MongoDbStudio.Models.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MongoDbStudio.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static async Task<JwtSecurityToken> GetJwtBearerTokenAsync(this HttpContext httpContext)
        {
            var token = await httpContext.GetTokenAsync(AuthorizationConstants.AccessToken);
            return new JwtSecurityToken(token);
        }
        public static Dictionary<string, object> FormatRequest(this HttpContext context, dynamic request = null, string version = null)
        {
            var exceptionParams = new Dictionary<string, object>
            {
                {"Request uri", new Uri(context.Request.GetDisplayUrl())},
                {"Request method", context.Request.Method},
                {"Request headers", context.Request.Headers},
            };

            if (request != null)
                exceptionParams.Add("Request body", JsonConvert.SerializeObject(request));

            if (!string.IsNullOrWhiteSpace(version))
                exceptionParams.Add("Assembly version", version);


            return exceptionParams;
        }

        public static Dictionary<string, object> FormatRequestBody(this HttpContext context, string version = null)
        {
            string request = context.GetBodyString();
            var exceptionParams = new Dictionary<string, object>
            {
                {"Request uri", new Uri(context.Request.GetDisplayUrl())},
                {"Request method", context.Request.Method},
                {"Request headers", context.Request.Headers},
            };

            if (!string.IsNullOrWhiteSpace(request))
                exceptionParams.Add("Request body", request);

            if (!string.IsNullOrWhiteSpace(version))
                exceptionParams.Add("Assembly version", version);

            return exceptionParams;
        }

        public static Dictionary<string, object> FormatRequestSerialized(this HttpContext context, string request = null, string version = null)
        {
            var exceptionParams = new Dictionary<string, object>
            {
                {"Request uri", new Uri(context.Request.GetDisplayUrl())},
                {"Request method", context.Request.Method},
                {"Request headers", context.Request.Headers},
            };

            if (!string.IsNullOrWhiteSpace(request))
                exceptionParams.Add("Request body", request);

            if (!string.IsNullOrWhiteSpace(version))
                exceptionParams.Add("Assembly version", version);

            return exceptionParams;
        }

        public static Dictionary<string, object> FormatResponseSerialized(this HttpContext context, string response = null, string version = null)
        {
            var exceptionParams = new Dictionary<string, object>
            {
                {"Response headers", context.Response.Headers},
            };

            if (!string.IsNullOrWhiteSpace(response))
                exceptionParams.Add("Response body", response);

            if (!string.IsNullOrWhiteSpace(version))
                exceptionParams.Add("Assembly version", version);

            return exceptionParams;
        }

        public static Dictionary<string, object> FormatResponse(this HttpContext context, dynamic response = null, string version = null)
        {
            var exceptionParams = new Dictionary<string, object>
            {
                {"Response headers", context.Response.Headers},
            };

            if (response != null)
                exceptionParams.Add("Response body", JsonConvert.SerializeObject(response));

            if (!string.IsNullOrWhiteSpace(version))
                exceptionParams.Add("Assembly version", version);

            return exceptionParams;
        }

        public static string GetCorrelation(this HttpContext context)
        {
            string item = context.GetRealCorrelation();

            if (string.IsNullOrWhiteSpace(item))
                return context.Request.Path;
            else
                return item;

        }

        public static string GetRealCorrelation(this HttpContext context)
        {
            return context.GetContextItem(ApiConstants.CorrelationKey);

        }

        public static string GetContextItem(this HttpContext context, string itemName)
        {
            if (string.IsNullOrWhiteSpace(itemName))
                return null;

            context.Items.TryGetValue(itemName, out var value);

            if (value == null)
                return null;
            else
                return value.ToString();

        }

        public static string GetParentCorrelation(this HttpContext context)
        {
            return context.Request?.Headers[ApiConstants.ParentCorrelationHeader];

        }

        public static string GetParentCorrelationToSend(this HttpContext context)
        {
            return context.GetContextItem(ApiConstants.ParentCorrelationHeader);

        }

        public static void SetParentCorrelationToSend(this HttpContext context, string parent = null)
        {
            if (string.IsNullOrWhiteSpace(parent))
                parent = GetParentCorrelation(context);
            if (string.IsNullOrWhiteSpace(parent))
                parent = GetCorrelation(context);

            context.Items[ApiConstants.ParentCorrelationHeader] = parent;
        }
        public static void SetCorrelation(this HttpContext context, string correlationId)
        {
            context.SetContextItem(ApiConstants.CorrelationKey, correlationId);
        }

        public static void RemoveCorrelation(this HttpContext context)
        {
            context.RemoveContextItem(ApiConstants.CorrelationKey);
        }

        public static void SetContextItem(this HttpContext context, string itemName, string value)
        {
            if (!string.IsNullOrWhiteSpace(itemName) && !string.IsNullOrWhiteSpace(value))
                context.Items[itemName] = value;
        }

        public static void RemoveContextItem(this HttpContext context, string itemName)
        {
            if (!string.IsNullOrWhiteSpace(itemName) && context.Items != null)
                context.Items.Remove(itemName);
        }

        public static bool HasClaims(this HttpContext HttpContext, params string[] claims)
        {
            if (!claims.Any())
                return false;

            var userClaims = HttpContext.User?.Claims;

            if (!userClaims.Any())
                return false;

            foreach (string claim in claims)
            {
                if (!string.IsNullOrWhiteSpace(claim))
                    if (!userClaims.Any(f => !string.IsNullOrWhiteSpace(f?.Type) && f.Type.Trim().ToUpper() == claim.Trim().ToUpper()))
                        return false;
            }

            return true;
        }

        public static bool HasClaimWithValue(this HttpContext HttpContext, string claim, string value)
        {
            if (string.IsNullOrWhiteSpace(claim) || string.IsNullOrWhiteSpace(value))
                return false;

            var userClaim = HttpContext.GetClaimValue(claim);

            if (string.IsNullOrWhiteSpace(userClaim))
                return false;

            if (userClaim.Trim().ToUpper() == value.Trim().ToUpper())
                return true;
            else
                return false;

        }

        public static Claim GetClaim(this HttpContext HttpContext, string claim)
        {
            if (string.IsNullOrWhiteSpace(claim))
                return null;

            return HttpContext.User?.Claims?.FirstOrDefault(x => x.Type.Trim().ToUpper() == claim.Trim().ToUpper());

        }

        public static string GetClaimValue(this HttpContext HttpContext, string claim)
        {

            return HttpContext.GetClaim(claim)?.Value;

        }

        public static string GetClientId(this HttpContext HttpContext)
        {
            var clientId = HttpContext.GetClaimValue(AuthorizationConstants.ClientIdOther);
            if (string.IsNullOrWhiteSpace(clientId))
                clientId = HttpContext.GetClaimValue(AuthorizationConstants.ClientId);
            return clientId;
        }

        public static string[] GetScopes(this HttpContext HttpContext)
        {
            var userScopes = HttpContext.GetClaimValue(AuthorizationConstants.Scope);

            if (userScopes == null)
                return null;

            var splittedUserScopes = userScopes.Split(" ");
            if (!splittedUserScopes.Any())
                return null;

            return splittedUserScopes;
        }
        public static bool HasScopes(this HttpContext HttpContext, params string[] scopes)
        {
            if (!scopes.Any())
                return false;

            var splittedUserScopes = HttpContext.GetScopes();

            if (splittedUserScopes == null)
                return false;

            foreach (string scope in scopes)
            {
                if (!string.IsNullOrWhiteSpace(scope))
                    if (!splittedUserScopes.Any(f => !string.IsNullOrWhiteSpace(f) && f.Trim().ToUpper() == scope.Trim().ToUpper()))
                        return false;
            }

            return true;
        }

        public static string GetBodyString(this HttpContext HttpContext)
        {
            string bodyText;
            try
            {
                var bodyStream = new StreamReader(HttpContext.Request.Body);
                bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
                bodyText = bodyStream.BaseStream.Length == 0 ? null : bodyStream.ReadToEnd();

            }
            catch
            {
                bodyText = null;
            }

            return bodyText;
        }

        public static JToken GetBodyObject(this HttpContext HttpContext)
        {
            string body = HttpContext.GetBodyString();
            return string.IsNullOrWhiteSpace(body) ? null : JToken.Parse(body);
        }

        public static T GetBodyObject<T>(this HttpContext HttpContext)
        {

            string body = HttpContext.GetBodyString();

            return string.IsNullOrWhiteSpace(body) ? default(T) : JsonConvert.DeserializeObject<T>(body);

        }

    }

}
