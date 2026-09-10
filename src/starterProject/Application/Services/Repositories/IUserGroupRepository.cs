using Core.Persistence.Repositories;
using Domain.Entities;

namespace Application.Services.Repositories;

public interface IUserGroupRepository
    : IAsyncRepository<UserGroup, int>,
        IRepository<UserGroup, int> { }
