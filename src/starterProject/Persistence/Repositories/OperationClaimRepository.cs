using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class OperationClaimRepository(BaseDbContext context)
    : EfRepositoryBase<OperationClaim, int, BaseDbContext>(context),
        IOperationClaimRepository { }
