using FluentValidation;

namespace OleAI.Backend.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(50).WithMessage("Ad en fazla 50 karakter olabilir.");

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("Soyad en fazla 50 karakter olabilir.");

            RuleFor(x => x.Username)
                .MaximumLength(50).WithMessage("Kullanıcı adı en fazla 50 karakter olabilir.")
                .Matches("^[a-zA-Z0-9_]*$").WithMessage("Kullanıcı adı sadece harf, rakam ve alt çizgi içerebilir.");

            RuleFor(x => x.BirthDate)
                .Must(BeAtLeast13YearsOld).When(x => x.BirthDate.HasValue)
                .WithMessage("Kullanıcılar en az 13 yaşında olmalıdır.");
        }

        private bool BeAtLeast13YearsOld(DateTime? birthDate)
        {
            if (!birthDate.HasValue) return true;

            var today = DateTime.Today;
            var age = today.Year - birthDate.Value.Year;
            if (birthDate.Value.Date > today.AddYears(-age)) age--;

            return age >= 13;
        }
    }
}
