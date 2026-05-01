using Microsoft.EntityFrameworkCore;
using MyProject.Infrastructure;

namespace MyProject.Infrastructure.Repositories.User;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Domain.Entities.User user)
    {
        await _context.Users.AddAsync(user);
    }

    public async Task<Domain.Entities.User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Domain.Entities.User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<List<Domain.Entities.User>> GetAllAsync()
    {
        return await _context.Users.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    public void Remove(Domain.Entities.User user)
    {
        user.IsDeleted = true;
        _context.Users.Update(user);
    }
}
