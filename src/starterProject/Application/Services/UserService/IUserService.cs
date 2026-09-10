using Domain.Dtos.Users;
using Domain.Entities;

namespace Application.Services.UserService;

public interface IUserService
{
    public Task<User?> GetByEmailAsync(string email);
    public Task<User> CreateUser(CreateUserDto createUser);
    public Task<User?> GetByIdAsync(int id);
}
