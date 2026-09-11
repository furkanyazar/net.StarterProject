using Application.Features.Auth.Commands.ForgotPassword;
using Application.Features.Auth.Commands.Login;
using Application.Features.Auth.Commands.RefreshToken;
using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.ResetPassword;
using Application.Features.Auth.Commands.RevokeToken;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration) : BaseController(configuration)
{
    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginCommand loginCommand)
    {
        loginCommand.IpAddress = GetIpAddress();
        LoggedResponse result = await Mediator.Send(loginCommand);

        if (result.RefreshToken is not null)
            SetRefreshTokenToCookie(result.RefreshToken);

        return Ok(result.ToHttpResponse());
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegisterCommand registerCommand)
    {
        registerCommand.IpAddress = GetIpAddress();
        registerCommand.SendMailDto = GetMailInfo();
        RegisteredResponse result = await Mediator.Send(registerCommand);

        if (result.RefreshToken is not null)
            SetRefreshTokenToCookie(result.RefreshToken);

        return Ok(result.AccessToken);
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> RefreshToken([FromQuery] string? refreshToken)
    {
        RefreshTokenCommand refreshTokenCommand = new()
        {
            RefreshToken = refreshToken ?? GetRefreshTokenFromCookies(),
            IpAddress = GetIpAddress(),
        };
        RefreshedTokenResponse result = await Mediator.Send(refreshTokenCommand);

        if (result.RefreshToken is not null)
            SetRefreshTokenToCookie(result.RefreshToken);

        return Ok(result.ToHttpResponse());
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> RevokeToken([FromQuery] string? refreshToken)
    {
        RevokeTokenCommand revokeTokenCommand = new()
        {
            Token = refreshToken ?? GetRefreshTokenFromCookies(),
            IpAddress = GetIpAddress(),
        };
        RevokedTokenResponse result = await Mediator.Send(revokeTokenCommand);

        DeleteRefreshTokenFromCookies();

        return Ok(result);
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordCommand forgotPasswordCommand
    )
    {
        forgotPasswordCommand.SendMailDto = GetMailInfo();
        await Mediator.Send(forgotPasswordCommand);
        return Ok();
    }

    [HttpPut("[action]")]
    public async Task<IActionResult> ResetPassword(
        [FromBody] ResetPasswordCommand resetPasswordCommand
    )
    {
        resetPasswordCommand.IpAddress = GetIpAddress();
        resetPasswordCommand.SendMailDto = GetMailInfo();
        ResetPasswordResponse result = await Mediator.Send(resetPasswordCommand);

        if (result.RefreshToken is not null)
            SetRefreshTokenToCookie(result.RefreshToken);

        return Ok(result.ToHttpResponse());
    }
}
