using FluentValidation;

namespace OleAI.Backend.Application.Features.Recipes.Commands.CreateRecipe;

public class CreateRecipeCommandValidator : AbstractValidator<CreateRecipeCommand>
{
    public CreateRecipeCommandValidator()
    {
        RuleFor(x => x.CreateRecipeRequest.Title)
            .NotEmpty().WithMessage("Tarif adı boş olamaz.")
            .MaximumLength(100).WithMessage("Tarif adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.CreateRecipeRequest.MainImage)
            .NotNull().WithMessage("Tarifin ana kapak fotoğrafı zorunludur.");

        RuleFor(x => x.Ingredients)
            .NotEmpty().WithMessage("En az bir malzeme eklemelisiniz.");

        RuleFor(x => x.Steps)
            .NotEmpty().WithMessage("En az bir hazırlık adımı eklemelisiniz.");

        RuleForEach(x => x.Ingredients).ChildRules(ingredient =>
        {
            ingredient.RuleFor(i => i.IngredientId)
                .NotEmpty()
                .When(i => string.IsNullOrEmpty(i.RequestedName))
                .WithMessage("Kayıtlı bir malzeme seçilmeli veya yeni bir malzeme önerilmelidir.");

            ingredient.RuleFor(i => i.RequestedName)
                .NotEmpty()
                .When(i => !i.IngredientId.HasValue)
                .WithMessage("Kayıtlı bir malzeme seçilmeli veya yeni bir malzeme önerilmelidir.");

            ingredient.RuleFor(i => i.Amount).GreaterThan(0).WithMessage("Miktar 0'dan büyük olmalıdır.");
        });

        RuleForEach(x => x.Steps).ChildRules(step =>
        {
            step.RuleFor(s => s.StepNumber).GreaterThan(0);
            step.RuleFor(s => s.Instruction).NotEmpty().WithMessage("Adım açıklaması boş olamaz.");
        });
    }
}
