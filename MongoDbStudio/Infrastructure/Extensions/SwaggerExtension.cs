﻿using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDbStudio.Infrastructure.Extensions
{
    public class CustomOperationFilter : IOperationFilter
    {
        private readonly IConfiguration _configuration;

        const string captureName = "routeParameter";

        public CustomOperationFilter(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            bool.TryParse(_configuration["Globalization:Swagger:EnableAcceptLanguageParam"], out bool enablesAccLang);
            if (enablesAccLang)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "accept-language",
                    In = ParameterLocation.Header,
                    Required = false

                });
            }

            var httpMethodAttributes = context.MethodInfo
              .GetCustomAttributes(true)
              .OfType<Microsoft.AspNetCore.Mvc.RouteAttribute>();

            var httpMethodWithOptional = httpMethodAttributes?.FirstOrDefault(m => m.Template != null && m.Template.Contains("?"));
            if (httpMethodWithOptional == null)
                return;

            string regex = $"{{(?<{captureName}>\\w+)\\?}}";

            var matches = System.Text.RegularExpressions.Regex.Matches(httpMethodWithOptional.Template, regex);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                var name = match.Groups[captureName].Value;

                var parameter = operation.Parameters.FirstOrDefault(p => p.In == ParameterLocation.Path && p.Name == name);
                if (parameter != null)
                {
                    parameter.AllowEmptyValue = true;
                    parameter.Required = false;
                    parameter.Schema.Nullable = true;
                }
            }

        }
    }
    public class SwaggerAddEnumDescriptions : IDocumentFilter
    {

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {

            // add enum descriptions to result models
            foreach (var property in swaggerDoc.Components.Schemas.Where(x => x.Value?.Enum?.Count > 0))
            {
                IList<IOpenApiAny> propertyEnums = property.Value.Enum;
                if (propertyEnums != null && propertyEnums.Count > 0)
                {
                    property.Value.Description += DescribeEnum(propertyEnums, property.Key);
                }
            }

            // add enum descriptions to input parameters
            foreach (var pathItem in swaggerDoc.Paths)
            {
                DescribeEnumParameters(pathItem.Value.Operations, swaggerDoc, context.ApiDescriptions, pathItem.Key);
            }
        }

        private void DescribeEnumParameters(IDictionary<OperationType, OpenApiOperation> operations, OpenApiDocument swaggerDoc, IEnumerable<ApiDescription> apiDescriptions, string path)
        {
            path = path.Trim('/');
            if (operations != null)
            {
                var pathDescriptions = apiDescriptions.Where(a => a.RelativePath == path);
                foreach (var oper in operations)
                {
                    var operationDescription = pathDescriptions.FirstOrDefault(a => a.HttpMethod.Equals(oper.Key.ToString(), StringComparison.InvariantCultureIgnoreCase));
                    foreach (var param in oper.Value.Parameters)
                    {
                        var parameterDescription = operationDescription.ParameterDescriptions.FirstOrDefault(a => a.Name == param.Name);
                        if (parameterDescription != null && TryGetEnumType(parameterDescription.Type, out Type enumType))
                        {
                            var paramEnum = swaggerDoc.Components.Schemas.FirstOrDefault(x => x.Key == enumType.Name);
                            if (paramEnum.Value != null)
                            {
                                param.Description += DescribeEnum(paramEnum.Value.Enum, paramEnum.Key);
                            }
                        }
                    }
                }
            }
        }

        bool TryGetEnumType(Type type, out Type enumType)
        {
            if (type == null)
            {
                enumType = null;
                return false;

            }
            if (type.IsEnum)
            {
                enumType = type;
                return true;
            }
            else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                if (underlyingType != null && underlyingType.IsEnum)
                {
                    enumType = underlyingType;
                    return true;
                }
            }
            else
            {
                Type underlyingType = GetTypeIEnumerableType(type);
                if (underlyingType != null && underlyingType.IsEnum)
                {
                    enumType = underlyingType;
                    return true;
                }
                else
                {
                    var interfaces = type.GetInterfaces();
                    foreach (var interfaceType in interfaces)
                    {
                        underlyingType = GetTypeIEnumerableType(interfaceType);
                        if (underlyingType != null && underlyingType.IsEnum)
                        {
                            enumType = underlyingType;
                            return true;
                        }
                    }
                }
            }

            enumType = null;
            return false;
        }

        Type GetTypeIEnumerableType(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                var underlyingType = type.GetGenericArguments()[0];
                if (underlyingType.IsEnum)
                {
                    return underlyingType;
                }
            }

            return null;
        }

        private Type GetEnumTypeByName(string enumTypeName)
        {
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => x.Name == enumTypeName);
        }

        private string DescribeEnum(IList<IOpenApiAny> enums, string proprtyTypeName)
        {
            List<string> enumDescriptions = new List<string>();
            var enumType = GetEnumTypeByName(proprtyTypeName);
            if (enumType == null)
                return null;

            foreach (OpenApiInteger enumOption in enums)
            {
                int enumInt = enumOption.Value;

                string name = Enum.GetName(enumType, enumInt);

                enumDescriptions.Add(string.Format("{0} = {1}", enumInt, $"{name}"));
            }

            return string.Join(", ", enumDescriptions.ToArray());
        }
    }
}
