using AutoMapper;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using MongoDbStudio.Interfaces;
using System;

namespace MongoDbStudio.Services
{
    public class InfrastructureService : IInfrastructureService
    {
        public IConfiguration Configuration { get; }
        public ILogEngineService LogEngine { get; }
        public IStringLocalizer<Resources> Localizer { get; }
        public IMapper Mapper { get; }

        public InfrastructureService(IConfiguration configuration, ILogEngineService logEngine, IStringLocalizer<Resources> localizer, IMapper mapper)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            LogEngine = logEngine ?? throw new ArgumentNullException(nameof(logEngine));
            Localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
    }
}
