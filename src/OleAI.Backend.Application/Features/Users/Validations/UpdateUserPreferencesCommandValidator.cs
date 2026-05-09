using FluentValidation;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserPreferences
{
    public class UpdateUserPreferencesCommandValidator : AbstractValidator<UpdateUserPreferencesCommand>
    {
        public UpdateUserPreferencesCommandValidator()
        {
            RuleFor(x => x.DietaryPreferenceIds)
                .NotNull().WithMessage("Beslenme tercihleri listesi boş olamaz.");

            RuleForEach(x => x.DietaryPreferenceIds)
                .NotEmpty().WithMessage("Beslenme tercihi ID'si geçersiz.");
        }
    }
}
