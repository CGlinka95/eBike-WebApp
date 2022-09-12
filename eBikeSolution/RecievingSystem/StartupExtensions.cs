
#region Additional Namespaces
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecievingSystem.BLL;
using RecievingSystem.DAL;
#endregion

namespace RecievingSystem
{
    public static class StartupExtensions
    {
        public static void AddRecievingDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<RecievingDbContext>(options);
            services.AddTransient<PurchaseOrderServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<RecievingDbContext>();
                return new PurchaseOrderServices(context);
            });
            services.AddTransient<RecievingOrderDetailsServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<RecievingDbContext>();
                return new RecievingOrderDetailsServices(context);
            });
        }
    }
}