using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using MongoDbStudio.Infrastructure.Middlewares;
using System.Globalization;

namespace MongoDbStudio.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtension
    {
        public static void UseLocalizationMiddleware(this IApplicationBuilder app)
        {
            var supportedCultures = new[]
            {
                new CultureInfo("en"),
                new CultureInfo("it"),

                // TODO: aggiungere eventuali altre lingue da supportare
            };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("it"),

                // Formatting numbers, dates, etc.
                SupportedCultures = supportedCultures,

                // UI strings that we have localized.
                SupportedUICultures = supportedCultures
            });
        }

        public static IApplicationBuilder UseGlobalExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionMiddleware>();
        }
    }
}