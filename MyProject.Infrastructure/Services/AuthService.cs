using Application.Dtos;
using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using MyProject.Infrastructure.Helper;
using MyProject.Infrastructure.Repositories.User;

namespace MyProject.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly AppDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        AppDbContext dbContext,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (existing is not null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var user = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Role = "User",
            CreatedAt = DateTime.UtcNow
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        var userDto = ToUserDto(user);
        return new AuthResponse { Token = JwtHelper.GenerateToken(userDto), User = userDto };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user is null)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var userDto = ToUserDto(user);
        return new AuthResponse { Token = JwtHelper.GenerateToken(userDto), User = userDto };
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
