using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional namespaces
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesSystem.BLL;
using SalesSystem.DAL;
#endregion

namespace SalesSystem
{
    public static class StartupExtensions
    {
        public static void AddSalesDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<SalesDbContext>(options);
            services.AddTransient<CategoryServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<SalesDbContext>();
                return new CategoryServices(context);
            });
            services.AddTransient<PartServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<SalesDbContext>();
                return new PartServices(context);
            });
        }
    }
}
