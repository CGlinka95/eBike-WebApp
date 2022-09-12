#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicingSystem.ViewModels
{
    public class ServiceInfo
    {
        [Required(ErrorMessage = "Service description is required.")]
        [StringLength(100, ErrorMessage = "Service description is limited to 100 characters.")]
        public string ServiceDescription { get; set; }
        [Required(ErrorMessage = "Service hours is required.")]
        public decimal ServiceHours { get; set; }
        public string? CustomerComments { get; set; }
        [StringLength(10, ErrorMessage = "A valid coupon must be equal to 10 characters long.")]
        public string? CouponIDValue { get; set; }

        public ServiceInfo()
        {

        }

        public ServiceInfo(string serviceDescription, decimal serviceHours, string customerComments, string couponIDValue)
        {
            ServiceDescription = serviceDescription;
            ServiceHours = serviceHours;
            CustomerComments = customerComments;
            CouponIDValue = couponIDValue;
        }
    }
}
