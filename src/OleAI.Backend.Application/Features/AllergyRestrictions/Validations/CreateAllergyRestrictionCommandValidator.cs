using FluentValidation;

namespace OleAI.Backend.Application.Features.AllergyRestrictions.Commands.CreateAllergyRestriction;

public class CreateAllergyRestrictionCommandValidator : AbstractValidator<CreateAllergyRestrictionCommand>
{
    public CreateAllergyRestrictionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Alerji kısıtı adı boş bırakılamaz.")
            .MaximumLength(100).WithMessage("Alerji kısıtı adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

        RuleFor(x => x.ExcludedIngredientIds)
            .NotNull().WithMessage("Hariç tutulan malzemeler listesi null olamaz.");

        RuleFor(x => x.ExcludedCategoryIds)
            .NotNull().WithMessage("Hariç tutulan kategoriler listesi null olamaz.");
    }
}
