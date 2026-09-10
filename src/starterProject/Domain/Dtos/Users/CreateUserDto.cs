using Core.Application.Dtos;

namespace Domain.Dtos.Users;

public class CreateUserDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }

    public CreateUserDto()
    {
        Email = string.Empty;
        Password = string.Empty;
    }

    public CreateUserDto(string email, string password)
    {
        Email = email;
        Password = password;
    }
}
