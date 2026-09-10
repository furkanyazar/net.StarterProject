using Application.Features.Auth.Commands.Register;
using Application.Features.Auth.Commands.RevokeToken;
using AutoMapper;
using Domain.Dtos.Users;
using Domain.Entities;

namespace Application.Features.Auth.Profiles;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Core.Security.Entities.RefreshToken<Guid, int>, RefreshToken>().ReverseMap();

        CreateMap<RegisterCommand, CreateUserDto>().ReverseMap();

        CreateMap<RefreshToken, RevokedTokenResponse>().ReverseMap();
    }
}
