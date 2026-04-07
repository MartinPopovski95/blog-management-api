using BlogManagementApi.DataAccess.Repositories.Abstractions;
using BlogManagementApi.DataAccess.Repositories.Implementations;
using BlogManagementApi.Services.Abstractions;
using BlogManagementApi.Services.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BlogManagementApi.Helpers.DependencyInjectionHelper
{
    public static class DIHelper
    {
        public static IServiceCollection RegisterRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPostRepository, PostRepository>();
            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuditLogService, AuditLogService>();
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IAdminUserService, AdminUserService>();
            return services;
        }
    }
}
