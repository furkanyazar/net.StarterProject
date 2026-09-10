using FluentValidation;

namespace Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty();
        RuleFor(c => c.Email).EmailAddress();
        RuleFor(c => c.Password).NotEmpty();
        RuleFor(c => c.Password).MinimumLength(8);
        RuleFor(c => c.Password).Must(HaveAtLeastOneLetter);
        RuleFor(c => c.Password).Must(HaveAtLeastOneDigit);
    }

    private bool HaveAtLeastOneLetter(string password)
    {
        return password.Any(char.IsLetter);
    }

    private bool HaveAtLeastOneDigit(string password)
    {
        return password.Any(char.IsDigit);
    }
}
