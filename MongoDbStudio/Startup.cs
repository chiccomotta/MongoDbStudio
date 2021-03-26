using Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDbStudio.Infrastructure.Authentication;
using MongoDbStudio.Infrastructure.Authorization;
using MongoDbStudio.Infrastructure.Extensions;
using MongoDbStudio.Infrastructure.Managers;
using MongoDbStudio.Interfaces;
using MongoDbStudio.Models.Common;
using Newtonsoft.Json;
using NLogEngine;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace MongoDbStudio
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // LogEngine configuration and registration
            LogEngine.Setup(Configuration["LogEngine:Appid"], AppDomain.CurrentDomain);

            LogEngine.SendLog(LogLevels.DEBUG, "START!", null);

            services.AddSingleton<ILogEngineService, LogEngineService>();

            services.AddControllers()
                .ConfigureApiBehaviorOptions(c =>
                    c.SuppressModelStateInvalidFilter = true) // to explicit management model.isvalid
                .AddNewtonsoftJson(
                    options =>
                    {

                        //options.SerializerSettings.ContractResolver = new DefaultContractResolver(); //enable pascal case
                        options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                        options.SerializerSettings.Formatting = Formatting.Indented;
                    })
                .AddJsonOptions(opt => opt.JsonSerializerOptions.PropertyNamingPolicy = null)
                .AddDataAnnotationsLocalization   // model data annotation/validation using localization resources
                (
                    options =>
                    {
                        options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(Resources));
                    }
                );

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.Zero,
                    RequireAudience = true,
                    RequireExpirationTime = true,
                    RequireSignedTokens = true,
                    ValidIssuer = Configuration["Oauth:Authority"],
                    ValidAudience = Configuration["Oauth:Audience"],
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeyResolver = (token, secureToken, kid, _) =>
                    {
                        var issuerDiscoveryEndpoint = new Uri(Configuration["Oauth:DiscoveryEndpoint"]);
                        var publicKeys = RedHatSSO.GetIssuerPublicKeys(issuerDiscoveryEndpoint, (JwtSecurityToken)secureToken);
                        return publicKeys;
                    }
                };
                o.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = tv => Task.Run(() => Console.WriteLine(tv.SecurityToken.ToString())),
                    OnAuthenticationFailed = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        c.Response.ContentType = "application/json";

                        return c.Response.WriteAsync(JsonConvert.SerializeObject(new BaseResponse
                        {
                            Error = new Error()
                            {
                                ErrorCode = (int)HttpStatusCode.Forbidden,
                                ErrorMessage = c.Exception.ToString()
                            },
                            RequestStatus = RequestStatus.KO.ToString()
                        }
                        ));
                    },
                    OnForbidden = c =>
                    {
                        c.NoResult();
                        c.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        c.Response.ContentType = "application/json";

                        return c.Response.WriteAsync(JsonConvert.SerializeObject(new BaseResponse
                        {
                            Error = new Error()
                            {
                                ErrorCode = (int)HttpStatusCode.Forbidden,
                                ErrorMessage = HttpStatusCode.Forbidden.ToString()
                            },
                            RequestStatus = RequestStatus.KO.ToString()
                        }
                        ));
                    }
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("HasValidAudience", policy => policy.Requirements.Add(new HasScopeRequirement(Configuration["Oauth:Audience"])));
                options.AddPolicy("CanCreate", policy => policy.Requirements.Add(new HasScopeRequirement("create")));
                options.AddPolicy("CanUpdate", policy => policy.Requirements.Add(new HasScopeRequirement("update")));
                options.AddPolicy("CanRead", policy => policy.Requirements.Add(new HasScopeRequirement("read")));
                options.AddPolicy("CanDelete", policy => policy.Requirements.Add(new HasScopeRequirement("delete")));
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "EA.NCOx.Net5Template",
                    Version = "v1",
                    Description = "<b>Template .NET 5 for Web API</b>"
                });

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.DocumentFilter<SwaggerAddEnumDescriptions>(); //enums custom management

                c.OperationFilter<CustomOperationFilter>();

                c.AddSecurityDefinition("JWT-Auth", new OpenApiSecurityScheme()
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Type = SecuritySchemeType.Http,
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "JWT-Auth" }
                        },
                        new string[]{}
                    }
                });
            });

            services.AddLocalization(o =>
            {
                // Mettiamo le risorse nella cartella Resources in un assembly separato
                o.ResourcesPath = "Resources";
            });

            // Validators
            services.AddValidators();

            // EntityFramework NCOx DbContext
            services.AddNCOxDbContext(Configuration);

            // EntityFramework NCOx DbContext
            services.AddNCO2DbContext(Configuration);

            // MongoDb Client
            services.AddSingleton<IMongoClient>(_ =>
                new MongoClient(Configuration.GetSection("MONGODB_CONNECTION_STRING").Value));
            
            services.AddHttpContextAccessor();
            services.AddHttpClient();
            services.AddAutoMapper();

            services.AddBusinessServices(Configuration);

            //authorization
            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddSingleton<IAuthorizationHandler, HasCustomClaimHandler>();

            Configuration["AssemblyVersion"] = typeof(Startup)?.Assembly?.GetName()?.Version?.NullableToString();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors(cpb =>
            {
                cpb.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.UseGlobalExceptionMiddleware();

            app.UseLocalizationMiddleware();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Microservice");
                c.RoutePrefix = string.Empty;
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
