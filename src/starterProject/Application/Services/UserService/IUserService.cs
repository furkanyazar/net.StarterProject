using Domain.Dtos.Mail;
using Domain.Dtos.Users;
using Domain.Entities;

namespace Application.Services.UserService;

public interface IUserService
{
    public Task<User> AddUser(User user);
    public Task<User> CreateUser(CreateUserDto createUser);
    public Task<User?> GetByEmail(string email);
    public Task<User?> GetById(int id);
    public Task SendRegisterMailToUserEmail(User user, SendMailDto sendMailDto);
    public Task SendResetPasswordMailToUserEmail(User user, SendMailDto sendMailDto);
    public Task<User> UpdateUser(User user);
}
