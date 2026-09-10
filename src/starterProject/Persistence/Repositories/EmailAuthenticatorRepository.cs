using Application.Services.Repositories;
using Core.Persistence.Repositories;
using Domain.Entities;
using Persistence.Contexts;

namespace Persistence.Repositories;

public class EmailAuthenticatorRepository(BaseDbContext context)
    : EfRepositoryBase<EmailAuthenticator, Guid, BaseDbContext>(context),
        IEmailAuthenticatorRepository { }
