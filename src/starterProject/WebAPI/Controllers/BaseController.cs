using Core.Security.Extensions;
using Core.Security.JWT;
using Domain.Dtos.Mail;
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
    private readonly WebAPIConfiguration _webApiConfiguration;

    public BaseController(IConfiguration configuration)
    {
        const string tokenOptionsConfigurationSection = "TokenOptions";
        _tokenOptions =
            configuration.GetSection(tokenOptionsConfigurationSection).Get<TokenOptions>()
            ?? throw new NullReferenceException(
                $"\"{tokenOptionsConfigurationSection}\" section cannot found in configuration"
            );

        const string webApiConfigurationSection = "WebAPIConfiguration";
        _webApiConfiguration =
            configuration.GetSection(webApiConfigurationSection).Get<WebAPIConfiguration>()
            ?? throw new NullReferenceException(
                $"\"{webApiConfigurationSection}\" section cannot found in configuration"
            );
    }

    protected string GetIpAddress()
    {
        bool isForwardedForHeaderPresent = Request.Headers.TryGetValue(
            "X-Forwarded-For",
            out StringValues ipAddress
        );
        if (isForwardedForHeaderPresent)
            return ipAddress.ToString();

        ipAddress = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        return ipAddress.ToString() ?? string.Empty;
    }

    protected string? GetLocale()
    {
        bool queryHasLocale = Request.Query.TryGetValue("locale", out StringValues locale);
        if (queryHasLocale)
            return locale.ToString();

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

    protected SendMailDto GetMailInfo()
    {
        SendMailDto sendMailDto = new()
        {
            AppDomain = _webApiConfiguration.AppDomain,
            AppName = _webApiConfiguration.AppName,
            Locale = GetLocale(),
        };
        return sendMailDto;
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

    protected string GetRefreshTokenFromCookies()
    {
        return Request.Cookies["refreshToken"] ?? string.Empty;
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
