using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserRepository(BaseDbContext context)
    : EfRepositoryBase<User, int, BaseDbContext>(context),
        IUserRepository { }
