using FluentValidation;
using OleAI.Backend.Application.Features.Recipes.Dtos.Requests;

namespace OleAI.Backend.Application.Features.Recipes.Commands.UpdateRecipe;

public class UpdateRecipeCommandValidator : AbstractValidator<UpdateRecipeCommand>
{
    public UpdateRecipeCommandValidator()
    {
        RuleFor(x => x.Request.Title)
            .NotEmpty().WithMessage("Tarif adı boş olamaz.")
            .MaximumLength(100).WithMessage("Tarif adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Request.Servings)
            .GreaterThan(0).WithMessage("Porsiyon 0'dan büyük olmalıdır.");

        RuleFor(x => x.Request.PrepTime)
            .GreaterThanOrEqualTo(0).WithMessage("Hazırlık süresi negatif olamaz.");

        RuleFor(x => x.Request.CookTime)
            .GreaterThanOrEqualTo(0).WithMessage("Pişirme süresi negatif olamaz.");

        RuleFor(x => x.Request.Difficulty)
            .IsInEnum().WithMessage("Geçerli bir zorluk derecesi seçilmelidir.");

        RuleFor(x => x.Request.CategoryIds)
            .Must(c => c == null || c.Count <= 3).WithMessage("En fazla 3 kategori seçebilirsiniz.");

        RuleFor(x => x.Request.TagIds)
            .Must(t => t == null || t.Count <= 5).WithMessage("En fazla 5 etiket seçebilirsiniz.");

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
