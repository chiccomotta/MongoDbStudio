using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MongoDbStudio.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MongoDbStudio.Services
{
    public class ServiceBase
    {
        protected readonly IConfiguration Configuration;
        protected readonly ILogEngineService LogEngine;
        protected readonly IMapper Mapper;
        protected readonly IHttpContextAccessor HttpContextAccessor;
        protected readonly IHttpClientFactory HttpClientFactory;
        //protected readonly PublicApiConfiguration PublicApiConfiguration;

        public ServiceBase(IInfrastructureService infrastructure, IHttpContextAccessor httpContextAccessor, IHttpClientFactory httpClientFactory)
        {
            Configuration = infrastructure.Configuration;
            LogEngine = infrastructure.LogEngine;
            Mapper = infrastructure.Mapper;
            HttpContextAccessor = httpContextAccessor;
            //PublicApiConfiguration = infrastructure.OptionsPublicApiConfiguration.Value;
            HttpClientFactory = httpClientFactory;
        }

        //protected async Task<string> GetPublicApiTokenAsync()
        //{
        //    using var httpClient = HttpClientFactory.CreateClient();
        //    var request = new Dictionary<string, string>()
        //    {
        //        {"audience", PublicApiConfiguration.Audience},
        //        {"client_id", PublicApiConfiguration.ClientId},
        //        {"client_secret", PublicApiConfiguration.ClientSecret},
        //        {"scope", string.Join(' ', PublicApiConfiguration.Scopes)},
        //        {"grant_type", Constants.OauthParams.ClientCredentialGrant}
        //    };

        //    var response = await httpClient.PostAsync(PublicApiConfiguration.TokenEndPoint, new FormUrlEncodedContent(request));
        //    var json = await response.Content.ReadAsStringAsync();
        //    var token = JToken.Parse(json);

        //    return (token[Constants.OauthParams.AccessToken] ?? throw new InvalidOperationException("Public Api Token not found")).Value<string>();
        //}
    }
}
