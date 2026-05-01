using Domain.Entities;

namespace MyProject.Infrastructure.Repositories.User;

public interface IUserRepository
{
    Task AddAsync(Domain.Entities.User user);
    Task<List<Domain.Entities.User>> GetAllAsync();
    Task<Domain.Entities.User?> GetByIdAsync(Guid id);
    Task<Domain.Entities.User?> GetByEmailAsync(string email);
    void Remove(Domain.Entities.User user);
}
