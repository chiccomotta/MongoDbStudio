using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using MongoDbStudio.Infrastructure.Extensions;
using MongoDbStudio.Models;
using MongoDbStudio.Models.Common;
using NLogEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MongoDbStudio.Infrastructure.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private string body;

        public GlobalExceptionMiddleware(RequestDelegate _next)
        {
            next = _next;
        }

        public async Task Invoke(HttpContext context)
        {
            PerformanceCounter performance = null;

            try
            {
                performance = new PerformanceCounter(context.Request.Path);
                context.Request.EnableBuffering();

                // Leave the body open so the _next middleware can read it.
                using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, false, 1024, true))
                {
                    // Leggo il body
                    body = await reader.ReadToEndAsync();

                    // Reset the request body stream position so the _next middleware can read it
                    context.Request.Body.Position = 0;
                }

                await next(context);
            }
            catch (Exception ex)
            {
                var correlation = context.GetCorrelation();

                var exceptionParams = new Dictionary<string, object>
                {
                    //{"Request uri", new Uri(new Uri(context.Request.Host.ToUriComponent()), context.Request.Path)},
                    {"Request uri", new Uri(context.Request.GetDisplayUrl())},
                    {"Request method", context.Request.Method},
                    {"Request headers", context.Request.Headers},
                    {"Request body", body}
                };

                // Log exception e request
                LogEngine.SendLog(LogLevels.ERROR, correlation, ex.ToString(), false, exceptionParams);

                // Ritorno un messaggio di risposta standard con i dettagli dell'errore.
                await HandleExceptionAsync(context, ex);
            }
            finally
            {
                performance?.Stop();
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new UnhandledExceptionResponse
            {
                StatusCode = context.Response.StatusCode,
                Message = ex.Message,
                InnerExceptionMessage = ex.InnerException?.Message,
                ExceptionInfo = ExceptionInfo.GetStackTrace(ex),
            }.ToString());
        }
    }
}