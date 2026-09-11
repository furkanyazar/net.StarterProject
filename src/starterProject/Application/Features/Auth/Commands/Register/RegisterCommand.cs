using Application.Features.Auth.Rules;
using Application.Services.AuthService;
using Application.Services.UserService;
using AutoMapper;
using Core.Security.JWT;
using Domain.Dtos.Mail;
using Domain.Dtos.Users;
using Domain.Entities;
using MediatR;

namespace Application.Features.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisteredResponse>
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string IpAddress { get; set; }
    public SendMailDto SendMailDto { get; set; } = default!;

    public RegisterCommand()
    {
        Email = string.Empty;
        Password = string.Empty;
        IpAddress = string.Empty;
    }

    public RegisterCommand(string email, string password, string ipAddress, SendMailDto sendMailDto)
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
            User addedUser = await userService.AddUser(createdUser);

            await userService.SendRegisterMailToUserEmail(addedUser, request.SendMailDto);

            AccessToken createdAccessToken = await authService.CreateAccessToken(addedUser);

            Domain.Entities.RefreshToken createdRefreshToken = await authService.CreateRefreshToken(
                addedUser,
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
