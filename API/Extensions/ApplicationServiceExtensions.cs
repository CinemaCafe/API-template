using _UniTechService.Interfaces;
using _UniTechService.Services;
using API._RepoService.Interfaces;
using API._RepoService.Services;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IUniService, UniService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddCors();

            return services;
        }
    }
}