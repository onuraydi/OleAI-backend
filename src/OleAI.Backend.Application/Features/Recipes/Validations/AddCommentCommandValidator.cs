using FluentValidation;

namespace OleAI.Backend.Application.Features.Recipes.Commands.AddComment;

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(c => c.Content)
            .NotEmpty().WithMessage("Yorum içeriği boş bırakılamaz.")
            .MinimumLength(2).WithMessage("Yorum çok kısa.")
            .MaximumLength(500).WithMessage("Yorum maksimum 500 karakter olabilir.");
    }
}
