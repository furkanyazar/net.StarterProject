using Application.Features.Auth.Constants;
using FluentValidation;

namespace Application.Features.Auth.Commands.Register;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().WithErrorCode(AuthErrorCodes.EmailRequired);
        RuleFor(c => c.Email).EmailAddress().WithErrorCode(AuthErrorCodes.EmailType);
        RuleFor(c => c.Password).NotEmpty().WithErrorCode(AuthErrorCodes.PasswordRequired);
        RuleFor(c => c.Password).MinimumLength(8).WithErrorCode(AuthErrorCodes.PasswordMinLength);
        RuleFor(c => c.Password)
            .Must(HaveAtLeastOneLetter)
            .WithErrorCode(AuthErrorCodes.PasswordAtLeastLetter);
        RuleFor(c => c.Password)
            .Must(HaveAtLeastOneDigit)
            .WithErrorCode(AuthErrorCodes.PasswordAtLeastDigit);
    }

    private bool HaveAtLeastOneLetter(string? password)
    {
        return password != null && password.Any(char.IsLetter);
    }

    private bool HaveAtLeastOneDigit(string? password)
    {
        return password != null && password.Any(char.IsDigit);
    }
}
