
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace MongoDbStudio.Infrastructure.Validators
{
    //public class CreateDossierDtoValidator : AbstractValidator<CreateDossierDto>
    //{
    //    public CreateDossierDtoValidator(IStringLocalizer<Resources> localizer)
    //    {
    //        RuleFor(c => c.Step1).NotNull().WithMessage(localizer["step1.required"]).DependentRules(
    //            () =>
    //            {
    //                RuleFor(c => c.Step1.FirstName).NotEmpty().WithMessage(localizer["name.required"]);
    //                RuleFor(c => c.Step1.LastName).NotEmpty().WithMessage(localizer["surname.required"]);
    //                RuleFor(c => c.Step1.Phone).NotEmpty().WithMessage(localizer["phone.required"]);
    //                RuleFor(c => c.Step1.Phone).Matches(@"^\+?[0-9]+$").WithMessage(localizer["phone.badformat"]);
    //                //RuleFor(c => c.Step1.FiscalCode)
    //                //    .Must(Utilities.IsFiscalCodeOk).WithMessage(localizer["fiscalcode.badformat"])
    //                //    .When(c => !string.IsNullOrEmpty(c.Step1.FiscalCode));
    //            });
    //    }
    //}
}