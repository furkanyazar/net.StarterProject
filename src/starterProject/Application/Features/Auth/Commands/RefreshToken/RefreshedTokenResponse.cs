using Core.Application.Responses;
using Core.Security.JWT;

namespace Application.Features.Auth.Commands.RefreshToken;

public class RefreshedTokenResponse : IResponse
{
    public AccessToken? AccessToken { get; set; }
    public Domain.Entities.RefreshToken? RefreshToken { get; set; }

    public RefreshedHttpResponse ToHttpResponse()
    {
        return new() { AccessToken = AccessToken };
    }

    public class RefreshedHttpResponse
    {
        public AccessToken? AccessToken { get; set; }
    }
}
