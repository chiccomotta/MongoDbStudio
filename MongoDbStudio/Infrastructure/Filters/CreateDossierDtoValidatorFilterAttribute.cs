using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc.Filters;
using MongoDbStudio.Infrastructure.Extensions;
using System.Linq;

namespace MongoDbStudio.Infrastructure.Filters
{
    public class CreateDossierDtoValidatorFilterAttribute : ActionFilterAttribute
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    var validator = (IValidator<CreateDossierDto>)context.HttpContext.RequestServices.GetService(typeof(IValidator<CreateDossierDto>));

        //    // Se il model binding fallisce
        //    if (!context.ModelState.IsValid)
        //    {
        //        context.Result = context.ModelState.GetBadRequestObjectResult();
        //        return;
        //    }

        //    // Validazione del modello
        //    foreach (var argument in context.ActionArguments.Values.Where(v => v is CreateDossierDto))
        //    {
        //        var model = argument as CreateDossierDto;
        //        validator.Validate(model).AddToModelState(context.ModelState, null);

        //        if (!context.ModelState.IsValid)
        //        {
        //            context.Result = context.ModelState.GetBadRequestObjectResult();
        //            return;
        //        }
        //    }
        //}
    }
}
