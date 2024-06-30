using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLocations.Core.Location;
using MyLocations.Core.Shared;
using MyLocations.Core.User;
using MyLocations.Infrastructure.Persistence;
using MyLocations.Infrastructure.Services.Flickr;
using MyLocations.Infrastructure.Services.FourSquare;

namespace MyLocations.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Persistence
            services.AddDbContext<ApplicationDbContext>(builder => builder.UseSqlServer(configuration.GetConnectionString("MyLocations")));

            services
                .AddScoped<ISettingsRepository, SettingsRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUnitOfWork, UnitOfWork>();

            // Third party services and external Apis
            services
                .AddHttpClient()
                .AddScoped<ISearchImageService, FlickrService>()
                .AddScoped<ISearchLocationService, FourSquareService>();

            // Identity

            return services;
        }
    }
}
