using Microsoft.AspNetCore.Identity;
using AppSecurity.Models;

namespace eBikeWebApp.Data
{
    public class ApplicationUser : IdentityUser, IIdentifyEmployee
    {
        public int? EmployeeId { get; set; }
    }
}
