using Application.Features.Auth.Constants;
using FluentValidation;

namespace Application.Features.Auth.Commands.ForgotPassword;

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().WithErrorCode(ErrorCodes.EmailRequired);
        RuleFor(c => c.Email).EmailAddress().WithErrorCode(ErrorCodes.EmailType);
    }
}
