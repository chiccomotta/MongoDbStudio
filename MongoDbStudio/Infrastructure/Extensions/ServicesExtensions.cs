using AutoMapper;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDbStudio.Infrastructure.Validators;
using MongoDbStudio.Interfaces;
using MongoDbStudio.Services;
using System;

namespace MongoDbStudio.Infrastructure.Extensions
{
    public static class ServicesExtensions
    {
        public static void AddNCO2DbContext(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<NCO2DbContext>(
            //    options => options.UseSqlServer(
            //        configuration.GetSection("NCO2_CONNECTION_STRING").Value));
        }

        public static void AddNCOxDbContext(this IServiceCollection services, IConfiguration configuration)
        {
            //services.AddDbContext<NCOxDbContext>(
            //    options => options.UseSqlServer(
            //        configuration.GetSection("NCOX_CONNECTION_STRING").Value));
        }

        public static void AddAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        public static void AddValidators(this IServiceCollection services)
        {
            //services.AddTransient<IValidator<CreateDossierDto>, CreateDossierDtoValidator>();
        }

        public static void AddBusinessServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Options
            //services.Configure<NoteServiceConfiguration>(configuration.GetSection("NoteServiceConfiguration"));
            //services.Configure<PublicApiConfiguration>(configuration.GetSection("PublicApiConfiguration"));
            //services.Configure<MediaApiConfiguration>(configuration.GetSection("MediaApiConfiguration"));

            // Business services
            services.AddScoped<IInfrastructureService, InfrastructureService>();
            //services.AddScoped<ISiscosService, SiscosService>();
            //services.AddScoped<INotesService, NotesService>();
            //services.AddScoped<IMediaService, MediaService>();
        }
    }
}