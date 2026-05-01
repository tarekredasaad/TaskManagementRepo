using Application.Dtos;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using MyProject.Infrastructure.Repositories.User;

namespace MyProject.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(IUserRepository userRepository, AppDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto?> GetByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        return user is null ? null : ToUserDto(user);
    }

    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(ToUserDto).ToList();
    }

    public async Task<UserDto> CreateByAdminAsync(RegisterRequest request, string role)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var existing = await _userRepository.GetByEmailAsync(email);
        if (existing is not null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = email,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return ToUserDto(user);
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || user.Role == "Admin")
        {
            return false;
        }

        _userRepository.Remove(user);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    private static UserDto ToUserDto(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };
}
