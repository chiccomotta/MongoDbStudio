using AutoMapper;
using Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

namespace MongoDbStudio.Interfaces
{
    public interface IInfrastructureService
    {
        IConfiguration Configuration { get; }
        ILogEngineService LogEngine { get; }
        IStringLocalizer<Resources> Localizer { get; }
        IMapper Mapper { get; }
        //IOptions<PublicApiConfiguration> OptionsPublicApiConfiguration { get; }
    }
}
