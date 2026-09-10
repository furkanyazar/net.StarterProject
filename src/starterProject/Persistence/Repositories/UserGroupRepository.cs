using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserGroupRepository(BaseDbContext context)
    : EfRepositoryBase<UserGroup, int, BaseDbContext>(context),
        IUserGroupRepository { }
