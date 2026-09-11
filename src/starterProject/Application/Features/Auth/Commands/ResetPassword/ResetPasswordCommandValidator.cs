using FluentValidation;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
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
