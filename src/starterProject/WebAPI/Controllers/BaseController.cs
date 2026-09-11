using Core.Security.Extensions;
using Core.Security.JWT;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

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

    private readonly TokenOptions _tokenOptions;

    public BaseController(IConfiguration configuration)
    {
        const string tokenOptionsConfigurationSection = "TokenOptions";
        _tokenOptions =
            configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException(
                $"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration"
            );
    }

    protected string? GetIpAddress()
    {
        bool isForwardedForHeaderPresent = Request.Headers.TryGetValue(
            "X-Forwarded-For",
            out StringValues ipAddress
        );
        if (isForwardedForHeaderPresent)
            return ipAddress.ToString();

        ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        return ipAddress;
    }

    protected string? GetLocale()
    {
        bool queryHasLocale = Request.Query.TryGetValue("locale", out StringValues locale);
        if (queryHasLocale)
            return locale.ToString();

        bool cookieHasLocale = Request.Cookies.TryGetValue("locale", out string? cookieLocale);
        if (cookieHasLocale)
            return cookieLocale;

        IList<StringWithQualityHeaderValue> acceptLanguages = Request
            .GetTypedHeaders()
            .AcceptLanguage;
        if (acceptLanguages.Count > 0)
            return acceptLanguages
                .OrderByDescending(x => x.Quality ?? 1)
                .Select(x => x.Value.ToString())
                .FirstOrDefault();

        return null;
    }

    protected int GetUserIdFromRequest()
    {
        bool parsed = int.TryParse(HttpContext.User.GetIdClaim(), out int userId);
        return parsed ? userId : 0;
    }

    protected ICollection<string> GetUserRolesFromRequest()
    {
        return HttpContext.User.GetRoleClaims() ?? [];
    }

    protected string? GetRefreshTokenFromCookies()
    {
        return Request.Cookies["refreshToken"];
    }

    protected void SetRefreshTokenToCookie(RefreshToken refreshToken)
    {
        CookieOptions cookieOptions = new()
        {
            Secure = true,
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenTTL),
        };
        Response.Cookies.Append(key: "refreshToken", refreshToken.Token, cookieOptions);
    }

    protected void DeleteRefreshTokenFromCookies()
    {
        Response.Cookies.Delete("refreshToken");
    }
}
