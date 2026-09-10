using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.UserService;
using AutoMapper;
using Core.Security.JWT;
using Domain.Dtos.Users;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisteredResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }

    public RegisterCommand()
    {
        Email = string.Empty;
        Password = string.Empty;
        IpAddress = string.Empty;
    }

    public RegisterCommand(string email, string password, string ipAddress)
    {
        Email = email;
        Password = password;
        IpAddress = ipAddress;
    }

    public class RegisterCommandHandler(
        AuthBusinessRules authBusinessRules,
        IAuthService authService,
        IUserService userService,
        IMapper mapper
    ) : IRequestHandler<RegisterCommand, RegisteredResponse>
    {
        public async Task<RegisteredResponse> Handle(
            RegisterCommand request,
            CancellationToken cancellationToken
        )
        {
            await authBusinessRules.EmailCanNotBeDuplicatedWhenRegistered(request.Email);

            CreateUserDto createUser = mapper.Map<CreateUserDto>(request);
            User createdUser = await userService.CreateUser(createUser);

            AccessToken createdAccessToken = await authService.CreateAccessToken(createdUser);

            Domain.Entities.RefreshToken createdRefreshToken = await authService.CreateRefreshToken(
                createdUser,
                request.IpAddress
            );
            Domain.Entities.RefreshToken addedRefreshToken = await authService.AddRefreshToken(
                createdRefreshToken
            );

            RegisteredResponse response = new()
            {
                AccessToken = createdAccessToken,
                RefreshToken = addedRefreshToken,
            };
            return response;
        }
    }
}
