using MyLocations.Core.Location;
using MyLocations.Core.User;
using MyLocations.Infrastructure;
using MyLocations.Web.Filters;
using MyLocations.Web.Middlewares;

namespace MyLocations
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews(opt => opt.Filters.Add<ModelStateValidationFilter>());

            builder.Services.AddRazorPages();

            builder.Services.AddScoped<LocationService>();

            builder.Services.AddScoped<UserService>();

            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseExceptionHandler("/Location/Error");

            app.UseHsts();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseMiddleware<ExceptionLoggingMiddleware>();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Location}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.Run();
        }
    }
}
