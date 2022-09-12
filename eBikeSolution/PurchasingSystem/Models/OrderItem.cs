using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchasingSystem.Models
{
    public class OrderItem
    {
        public int? PurchaseOrderDetailID { get; set; }
        public int PartID { get; set; }
        public int QuantityToOrder { get; set; }
        public decimal PurchasePrice { get; set; }
    }
}
