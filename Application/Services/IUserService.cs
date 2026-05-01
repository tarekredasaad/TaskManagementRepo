using Application.Dtos;

namespace Application.Services;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(Guid userId);
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto> CreateByAdminAsync(RegisterRequest request, string role);
    Task<bool> DeleteAsync(Guid userId);
}
