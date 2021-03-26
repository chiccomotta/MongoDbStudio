using AutoMapper;
using Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using MongoDbStudio.Interfaces;

namespace MongoDbStudio.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        protected readonly IConfiguration Configuration;
        protected readonly IStringLocalizer<Resources> Localizer;
        protected readonly ILogEngineService LogEngine;
        protected readonly IMapper Mapper;

        public ApiControllerBase(IInfrastructureService infrastructure)
        {
            Configuration = infrastructure.Configuration;
            LogEngine = infrastructure.LogEngine;
            Localizer = infrastructure.Localizer;
            Mapper = infrastructure.Mapper;
        }
    }
}