using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDbStudio.Models;
using MongoDbStudio.Models.Common;
using System.Collections.Generic;

namespace MongoDbStudio.Infrastructure.Extensions
{
    public static class ModelStateExtension
    {
        public static IEnumerable<ErrorMessage> GetValidationErrors(this ModelStateDictionary modelState)
        {
            var errors = new List<ErrorMessage>();
            foreach (var state in modelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    errors.Add(new ErrorMessage()
                    {
                        Field = state.Key,
                        Message = error.ErrorMessage
                    });
                }
            }

            return errors;
        }

        public static IActionResult GetBadRequestObjectResult(this ModelStateDictionary modelState)
        {
            var result = new BadRequestObjectResult(new ApiResponse
            {
                Status = ApiConstants.KO,
                Message = ApiConstants.BAD_REQUEST,
                Errors = modelState.GetValidationErrors()
            });

            return result;
        }
    }
}
