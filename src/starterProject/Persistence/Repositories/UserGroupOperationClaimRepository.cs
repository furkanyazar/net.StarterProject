using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class UserGroupOperationClaimRepository(BaseDbContext context)
    : EfRepositoryBase<UserGroupOperationClaim, Guid, BaseDbContext>(context),
        IUserGroupOperationClaimRepository { }
