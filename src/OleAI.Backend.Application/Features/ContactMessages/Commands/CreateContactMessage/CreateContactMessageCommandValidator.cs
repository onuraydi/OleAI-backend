using FluentValidation;

namespace OleAI.Backend.Application.Features.ContactMessages.Commands.CreateContactMessage
{
    public class CreateContactMessageCommandValidator : AbstractValidator<CreateContactMessageCommand>
    {
        public CreateContactMessageCommandValidator()
        {
            RuleFor(c => c.Name).NotEmpty().MinimumLength(2);
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.Department).NotEmpty();
            RuleFor(c => c.Message).NotEmpty().MinimumLength(10);
        }
    }
}
