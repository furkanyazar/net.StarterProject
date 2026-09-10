using Core.Persistence.Repositories;
using Domain.Entities;

namespace Application.Services.Repositories;

public interface IUserGroupOperationClaimRepository
    : IAsyncRepository<UserGroupOperationClaim, int>,
        IRepository<UserGroupOperationClaim, int> { }
