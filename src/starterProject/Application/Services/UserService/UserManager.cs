using Application.Services.MailQueueService;
using Application.Services.Repositories;
using Core.Security.Hashing;
using Domain.Dtos.Mail;
using Domain.Dtos.Users;
using Domain.Entities;

namespace Application.Services.UserService;

public class UserManager(IUserRepository userRepository, IMailQueueService mailQueueService)
    : IUserService
{
    public async Task<User> AddUser(User user)
    {
        User createdUser = await userRepository.AddAsync(user);
        return createdUser;
    }

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
            UserGroupId = 1,
        };
        return user;
    }

    public async Task<User?> GetByEmail(string email)
    {
        User? user = await userRepository.GetAsync(predicate: u => u.Email == email);
        return user;
    }

    public async Task<User?> GetById(int id)
    {
        User? user = await userRepository.GetAsync(predicate: u => u.Id == id);
        return user;
    }

    public async Task SendRegisterMailToUserEmail(User user, SendMailDto sendMailDto)
    {
        MailDto mailDto = new()
        {
            TemplateName = "Register",
            Locale = sendMailDto.Locale,
            ToList = [new(user.Name, user.Email)],
            Model = new
            {
                user.Name,
                sendMailDto.AppName,
                sendMailDto.AppDomain,
            },
        };

        await mailQueueService.SendAsync(mailDto);
    }

    public async Task SendResetPasswordMailToUserEmail(User user, SendMailDto sendMailDto)
    {
        MailDto mailDto = new()
        {
            TemplateName = "ResetPassword",
            Locale = sendMailDto.Locale,
            ToList = [new(user.Name, user.Email)],
            Model = new
            {
                user.Name,
                sendMailDto.AppName,
                sendMailDto.AppDomain,
            },
        };

        await mailQueueService.SendAsync(mailDto);
    }

    public async Task<User> UpdateUser(User user)
    {
        User updatedUser = await userRepository.UpdateAsync(user);
        return updatedUser;
    }
}
