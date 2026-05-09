using FluentValidation;

namespace OleAI.Backend.Application.Features.DietaryPreferences.Commands.UpdateDietaryPreference;

public class UpdateDietaryPreferenceCommandValidator : AbstractValidator<UpdateDietaryPreferenceCommand>
{
    public UpdateDietaryPreferenceCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Diyet tercihi ID'si geçersiz.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Diyet tercihi adı boş bırakılamaz.")
            .MaximumLength(100).WithMessage("Diyet tercihi adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

        RuleFor(x => x.ExcludedIngredientIds)
            .NotNull().WithMessage("Hariç tutulan malzemeler listesi null olamaz.");

        RuleFor(x => x.ExcludedCategoryIds)
            .NotNull().WithMessage("Hariç tutulan kategoriler listesi null olamaz.");
    }
}
