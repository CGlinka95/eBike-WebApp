using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using PurchasingSystem.DAL;
using PurchasingSystem.Models;
using PurchasingSystem.Entities;
#endregion

namespace PurchasingSystem.BLL
{
    public class VendorServices
    {
        #region Constructor and Context Dependency
        private readonly PurchasingDbContext _context;

        internal VendorServices(PurchasingDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public List<SelectionList> GetVendors()
        {
            List<SelectionList> vendors = _context.Vendors
                                           .Select(v => new SelectionList
                                           {
                                               ValueID = v.VendorID,
                                               DisplayText = v.VendorName
                                           })
                                           .ToList();
            return vendors;
        }

        public VendorInfo GetVendorByID(int vendorId)
        {
            if (vendorId < 0) throw new ArgumentNullException("No Vendor ID was supplied. Please try again.");

            // Check if the vendor exists
            VendorInfo? vendorExists = _context.Vendors
                                              .Where(v => v.VendorID == vendorId)
                                              .Select(v => new VendorInfo
                                              {
                                                  VendorID = v.VendorID,
                                                  VendorName = v.VendorName,
                                                  Phone = v.Phone,
                                                  City = v.City,
                                              })
                                              .FirstOrDefault();
            if (vendorExists == null)
            {
                throw new ArgumentException($"We could not find Vendor ({vendorId}) in our system. Please try again.");
            }

            return vendorExists;
        }

        public List<PurchaseOrderItem> GetVendorInventory(int vendorID, List<PurchaseOrderItem> currentOrder)
        {
            if (vendorID < 0) throw new ArgumentNullException("No Vendor ID was supplied. Please try again.");

            List<PurchaseOrderItem> inventory = _context.Parts
                                                        .Where(p => p.VendorID == vendorID)
                                                        .Select(p => new PurchaseOrderItem
                                                        {
                                                            PartID = p.PartID,
                                                            Description = p.Description,
                                                            PurchasePrice = p.PurchasePrice,
                                                            ReorderLevel = p.ReorderLevel,
                                                            QuantityOnHand = p.QuantityOnHand,
                                                            QuantityOnOrder = p.QuantityOnOrder
                                                        })
                                                        .ToList();

            List<PurchaseOrderItem> vendorInventory = inventory.Where(c => !currentOrder.Any(v => v.PartID == c.PartID)).ToList();

            return vendorInventory;
        }
        #endregion
    }
}
