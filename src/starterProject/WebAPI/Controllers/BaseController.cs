using Core.Security.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

public class BaseController : ControllerBase
{
    protected IMediator? Mediator =>
        _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
    private IMediator? _mediator;

    protected string? GetIpAddress()
    {
        bool requestHasForwardedForHeader = Request.Headers.TryGetValue(
            "X-Forwarded-For",
            out Microsoft.Extensions.Primitives.StringValues value
        );
        if (requestHasForwardedForHeader)
            return value;
        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }

    protected int GetUserIdFromRequest()
    {
        int userId = Convert.ToInt32(HttpContext.User.GetIdClaim());
        return userId;
    }
}
