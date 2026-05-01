using Application.Services;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyProject.Infrastructure.CashService;
using MyProject.Infrastructure.Repositories.Tasks;
using MyProject.Infrastructure.Repositories.User;
using MyProject.Infrastructure.Services;
using StackExchange.Redis;

namespace MyProject.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnection = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Missing connection string: DefaultConnection");
        var redisConnection = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Missing connection string: Redis");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(sqlConnection));

        services.AddSingleton<IConnectionMultiplexer>(_ =>
            ConnectionMultiplexer.Connect(redisConnection));
        services.AddScoped<RedisCacheService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        services.AddSingleton<ITaskProcessingQueue, TaskProcessingQueue>();
        services.AddHostedService<TaskBackgroundWorker>();

        return services;
    }
}
