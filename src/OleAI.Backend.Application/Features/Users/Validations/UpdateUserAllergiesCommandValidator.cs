using FluentValidation;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserAllergies
{
    public class UpdateUserAllergiesCommandValidator : AbstractValidator<UpdateUserAllergiesCommand>
    {
        public UpdateUserAllergiesCommandValidator()
        {
            RuleFor(x => x.AllergyRestrictionIds)
                .NotNull().WithMessage("Alerji kısıtları listesi boş olamaz.");

            RuleForEach(x => x.AllergyRestrictionIds)
                .NotEmpty().WithMessage("Alerji kısıtı ID'si geçersiz.");
        }
    }
}
