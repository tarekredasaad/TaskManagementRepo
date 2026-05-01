using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using MyProject.Infrastructure;

namespace MyProject.Infrastructure.Repositories.Tasks;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
    }

    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<TaskItem>> GetUserTasks(Guid userId)
    {
        return await _context.Tasks
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Priority)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> ExistsByTitleForDayAsync(Guid userId, string title, DateTime dayUtc)
    {
        var dayStart = dayUtc.Date;
        var dayEnd = dayStart.AddDays(1);
        return await _context.Tasks.AnyAsync(x =>
            x.UserId == userId &&
            x.Title == title &&
            x.CreatedAt >= dayStart &&
            x.CreatedAt < dayEnd);
    }

    public void Update(TaskItem task)
    {
        _context.Tasks.Update(task);
    }
}
