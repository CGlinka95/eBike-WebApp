using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PurchasingSystem.DAL;
using PurchasingSystem.BLL;
#endregion


namespace PurchasingSystem
{
    public static class StartupExtensions
    {
        public static void AddPurchasingDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<PurchasingDbContext>(options);

            services.AddTransient<PurchasingServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<PurchasingDbContext>();
                return new PurchasingServices(context);
            });
            services.AddTransient<VendorServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<PurchasingDbContext>();
                return new VendorServices(context);
            });
        }
    }
}
