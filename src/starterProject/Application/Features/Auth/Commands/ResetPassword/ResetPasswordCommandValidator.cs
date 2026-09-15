using Application.Features.Auth.Constants;
using FluentValidation;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
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
