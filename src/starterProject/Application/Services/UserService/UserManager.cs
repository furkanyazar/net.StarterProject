using Application.Services.Repositories;
using Core.Security.Hashing;
using Domain.Dtos.Users;
using Domain.Entities;

namespace Application.Services.UserService;

public class UserManager(IUserRepository userRepository) : IUserService
{
    public async Task<User> CreateUser(CreateUserDto createUser)
    {
        HashingHelper.CreatePasswordHash(
            createUser.Password,
            out byte[] passwordHash,
            out byte[] passwordSalt
        );

        User user = new()
        {
            Email = createUser.Email,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
        };

        User createdUser = await userRepository.AddAsync(user);
        return createdUser;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        User? user = await userRepository.GetAsync(predicate: u => u.Email == email);
        return user;
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        User? user = await userRepository.GetAsync(predicate: u => u.Id == id);
        return user;
    }
}
