using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.ViewModels
{
    public class PartsListBy
    {
        public int PartID { get; set; }
        [Required]
        [StringLength(40)]
        [Unicode(false)]
        public string Description { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SellingPrice { get; set; }
        public int QuantityOnHand { get; set; }
        public int ReorderLevel { get; set; }
        public int QuantityOnOrder { get; set; }
        public int CategoryID { get; set; }
        [Required]
        [StringLength(1)]
        [Unicode(false)]
        public string Refundable { get; set; }
        public bool Discontinued { get; set; }
        public int VendorID { get; set; }
    }
}
