using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional namespaces
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ServicingSystem.BLL;
using ServicingSystem.DAL;
#endregion

namespace ServicingSystem
{
    public static class ServicingExtensions
    {
        public static void AddServicingDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<ServicingDbContext>(options);
            services.AddTransient<CustomerServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ServicingDbContext>();
                return new CustomerServices(context);
            });
            services.AddTransient<VehicleServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ServicingDbContext>();
                return new VehicleServices(context);
            });
            services.AddTransient<JobServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ServicingDbContext>();
                return new JobServices(context);
            });
            services.AddTransient<StandardJobServices>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<ServicingDbContext>();
                return new StandardJobServices(context);
            });
        }
    }
}
