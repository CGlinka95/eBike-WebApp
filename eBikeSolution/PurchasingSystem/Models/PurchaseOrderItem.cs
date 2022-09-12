using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PurchasingSystem.Models
{
    public class PurchaseOrderItem
    {
        public int? PurchaseOrderDetailID { get; set; }
        public int PartID { get; set; }
        public string Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public int QuantityOnOrder { get; set; }
        public int? QuantityToOrder { get; set; }
        public int? Buffer { get { return (ReorderLevel - (QuantityOnHand + QuantityOnOrder)) * -1; } }
    }
}
