using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecievingSystem.ViewModels
{
    public class OutStandingOrder
    {
        public int PurchaseOrderID { get; set; }
        public DateTime? OrderDate { get; set; }
        public string VendorName { get; set; }
        public string PhoneNumber { get; set; }
        public int VendorID { get; set; }


    }
}
