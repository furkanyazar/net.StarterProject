using Application.Features.Auth.Constants;
using FluentValidation;

namespace Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().WithErrorCode(ErrorCodes.EmailRequired);
        RuleFor(c => c.Email).EmailAddress().WithErrorCode(ErrorCodes.EmailType);
        RuleFor(c => c.Password).NotEmpty().WithErrorCode(ErrorCodes.PasswordRequired);
        RuleFor(c => c.Password).MinimumLength(8).WithErrorCode(ErrorCodes.PasswordMinLength);
        RuleFor(c => c.Password)
            .Must(HaveAtLeastOneLetter)
            .WithErrorCode(ErrorCodes.PasswordAtLeastLetter);
        RuleFor(c => c.Password)
            .Must(HaveAtLeastOneDigit)
            .WithErrorCode(ErrorCodes.PasswordAtLeastDigit);
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
