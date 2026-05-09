using FluentValidation;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserDislikedIngredients;

public class UpdateUserDislikedIngredientsCommandValidator : AbstractValidator<UpdateUserDislikedIngredientsCommand>
{
    public UpdateUserDislikedIngredientsCommandValidator()
    {
        RuleFor(x => x.IngredientIds)
            .NotNull().WithMessage("Malzeme listesi null olamaz.");

        RuleForEach(x => x.IngredientIds)
            .NotEmpty().WithMessage("Malzeme ID'si geçersiz veya boş olamaz.");
    }
}
