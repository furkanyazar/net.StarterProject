using Core.Security.Extensions;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class BaseController : ControllerBase
{
    protected IMediator Mediator =>
        _mediator ??=
            HttpContext.RequestServices.GetService<IMediator>()
            ?? throw new InvalidOperationException(
                "IMediator cannot be retrieved from request services."
            );

    private IMediator? _mediator;

    protected string GetIpAddress()
    {
        string ipAddress = Request.Headers.TryGetValue(
            "X-Forwarded-For",
            out Microsoft.Extensions.Primitives.StringValues value
        )
            ? value.ToString()
            : HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString()
                ?? throw new InvalidOperationException(
                    "IP address cannot be retrieved from request."
                );
        return ipAddress;
    }

    protected int GetUserIdFromRequest()
    {
        bool parsed = int.TryParse(HttpContext.User.GetIdClaim(), out int userId);
        return parsed ? userId : 0;
    }

    protected ICollection<string> GetUserRolesFromRequest()
    {
        return HttpContext.User.GetRoleClaims() ?? Array.Empty<string>();
    }

    protected string GetRefreshTokenFromCookies()
    {
        return Request.Cookies["refreshToken"]
            ?? throw new ArgumentException("Refresh token is not found in request cookies.");
    }

    protected void SetRefreshTokenToCookie(RefreshToken refreshToken)
    {
        CookieOptions cookieOptions = new()
        {
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7),
        };
        Response.Cookies.Append(key: "refreshToken", refreshToken.Token, cookieOptions);
    }

    protected void DeleteRefreshTokenFromCookies()
    {
        Response.Cookies.Delete("refreshToken");
    }
}
