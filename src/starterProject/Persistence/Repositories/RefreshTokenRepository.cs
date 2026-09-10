using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class RefreshTokenRepository(BaseDbContext context)
    : EfRepositoryBase<RefreshToken, Guid, BaseDbContext>(context),
        IRefreshTokenRepository { }
