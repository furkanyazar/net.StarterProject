using Application.Features.Auth.Constants;
using FluentValidation;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
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
