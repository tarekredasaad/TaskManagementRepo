using System.Text;
using Domain.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyProject.Infrastructure;
using MyProject.Infrastructure.Services;
using TaskManagement.Middleware;
using TaskManagement.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .WriteTo.Console()
                 .Enrich.FromLogContext());

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UserState>(sp =>
{
    var accessor = sp.GetRequiredService<IHttpContextAccessor>();
    var user = accessor.HttpContext?.User;
    return new UserState
    {
        ID = user?.FindFirst(CustomClaimTypes.Id)?.Value ?? string.Empty,
        Role = user?.FindFirst(CustomClaimTypes.RoleId)?.Value
            ?? user?.FindFirst(CustomClaimTypes.Role)?.Value
            ?? user?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
            ?? string.Empty,
        Name = user?.FindFirst(CustomClaimTypes.Name)?.Value
            ?? user?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
            ?? string.Empty
    };
});
builder.Services.AddScoped<ControllerParameters>(sp => new ControllerParameters
{
    UserState = sp.GetRequiredService<UserState>(),
    HttpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>()
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Task Management API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. " +
                        "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
                        "\r\n\r\nExample: \"Bearer abcdefghijklmnopqrstuvwxyz\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,

            },
            new List<string>()
        }
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddAuthentication(opts =>
{
    opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = JwtSetting.Issuer,
        ValidateAudience = true,
        ValidAudience = JwtSetting.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero, // Reduce the default clock skew (allowable token time discrepancy)
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSetting.Key))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("TaskReadPolicy", policy =>
        policy.RequireAssertion(context =>
            context.User.IsInRole("Admin") || context.User.IsInRole("User")));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await DatabaseSeeder.SeedAsync(app.Services);

app.Run();
