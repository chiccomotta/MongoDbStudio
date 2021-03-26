using Microsoft.IdentityModel.Tokens;
using MongoDbStudio.Models.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;

namespace MongoDbStudio.Infrastructure.Authentication
{
    public static class PublicKeysCache
    {
        public static DateTime StartTime { get; set; } = DateTime.UtcNow;
        public static SecurityKey[] SecurityKeys { get; set; }
    }

    public static class RedHatSSO
    {
        public static IEnumerable<SecurityKey> GetIssuerPublicKeys(Uri issuerDiscoveryEndpoint, JwtSecurityToken token)
        {
            if ((DateTime.UtcNow - PublicKeysCache.StartTime).Hours < RedHat.ExpiredCache && PublicKeysCache.SecurityKeys != null)
            {
                return PublicKeysCache.SecurityKeys;
            }

            using (var httpClient = new HttpClient())
            {
                var oidcRealmConfig = httpClient.GetStringAsync(issuerDiscoveryEndpoint).Result;
                var oidcRealmConfigJSON = JToken.Parse(oidcRealmConfig);
                var certsConfig = httpClient.GetStringAsync(oidcRealmConfigJSON["jwks_uri"].Value<string>()).Result;
                var certsConfigJSON = JToken.Parse(certsConfig);
                var publicKeyParts = certsConfigJSON["keys"]
                    .Where(jt => jt["kid"].ToString() == (token).Header.Kid)
                    .Select(jt => new
                    {
                        KID = jt["kid"].ToString(),
                        KeyType = jt["kty"].ToString(),
                        KeyAlgorithm = jt["alg"].ToString(),
                        Base64EncodedModulus = jt["n"].ToString(),
                        Base64EncodedExponent = jt["e"].ToString(),
                    })
                    .FirstOrDefault();

                if (publicKeyParts == null)
                {
                    throw new ArgumentException(ApiMessages.RedHatError);
                }

                var rsa = new RSACryptoServiceProvider();
                rsa.ImportParameters(
                  new RSAParameters()
                  {
                      Modulus = FromBase64Url(publicKeyParts.Base64EncodedModulus),
                      Exponent = FromBase64Url(publicKeyParts.Base64EncodedExponent)
                  });

                PublicKeysCache.SecurityKeys = new SecurityKey[] { new RsaSecurityKey(rsa) };
                PublicKeysCache.StartTime = DateTime.UtcNow;
                return new SecurityKey[] { new RsaSecurityKey(rsa) };
            }
        }

        static byte[] FromBase64Url(string base64Url)
        {
            string padded = base64Url.Length % 4 == 0
                ? base64Url : base64Url + "====".Substring(base64Url.Length % 4);
            string base64 = padded.Replace("_", "/")
                                  .Replace("-", "+");
            return Convert.FromBase64String(base64);
        }
    }
}
