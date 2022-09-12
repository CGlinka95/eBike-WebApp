using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional namespaces
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AppSecurity.BLL;
using AppSecurity.DAL;
#endregion

namespace AppSecurity
{
    public static class SecurityExtensions
    {
        public static void AppSecurityBackendDependencies(this IServiceCollection services, Action<DbContextOptionsBuilder> options)
        {
            services.AddDbContext<AppSecurityDbContext>(options);

            services.AddTransient<SecurityService>((serviceProvider) =>
            {
                var context = serviceProvider.GetRequiredService<AppSecurityDbContext>();
                return new SecurityService(context);
            });
        }
    }
}
