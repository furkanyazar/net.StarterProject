using Core.Application.Responses;
using Core.Security.JWT;

namespace Application.Features.Auth.Commands.ResetPassword;

public class ResetPasswordResponse : IResponse
{
    public AccessToken? AccessToken { get; set; }
    public Domain.Entities.RefreshToken? RefreshToken { get; set; }

    public RegisteredHttpResponse ToHttpResponse()
    {
        return new() { AccessToken = AccessToken };
    }

    public class RegisteredHttpResponse
    {
        public AccessToken? AccessToken { get; set; }
    }
}
