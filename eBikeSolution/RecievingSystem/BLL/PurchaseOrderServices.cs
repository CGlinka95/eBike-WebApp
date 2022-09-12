using Microsoft.EntityFrameworkCore.ChangeTracking;
using RecievingSystem.DAL;
using RecievingSystem.Entities;
using RecievingSystem.ViewModels;
/// <summary>
/// Just for querying for Querying **OPEN** Purcahse Order and their details. 
/// </summary>
namespace RecievingSystem.BLL
{
    public class PurchaseOrderServices
    {
        private readonly RecievingDbContext _context;

        internal PurchaseOrderServices(RecievingDbContext context)
        {
            _context = context;
        }
        public List<OutStandingOrder> fetchOutStandingOrders()
        {
            var ordersQuery = _context.PurchaseOrders.Where(x => x.Closed == false).Select(x => new OutStandingOrder
            {
                PurchaseOrderID = x.PurchaseOrderID,
                OrderDate = x.OrderDate,
                VendorID = x.VendorID,
                VendorName = _context.Vendors.Where(y => y.VendorID == x.VendorID).Select(z => z.VendorName).FirstOrDefault() ?? "",
                PhoneNumber = _context.Vendors.Where(y => y.VendorID == x.VendorID).Select(z => z.Phone).FirstOrDefault() ?? "N/A"

            }); ;

            return ordersQuery.ToList();
        }

        public void clearUnOrderedItemsTable()

        {
            if (_context.PurchaseOrders.Any())
            {

                var deleteItems = _context.UnorderedPurchaseItemCarts;
                foreach (var item in deleteItems)
                {
                    EntityEntry<UnorderedPurchaseItemCart> cartEntry = _context.Entry(item);
                    cartEntry.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                }
                _context.SaveChanges();
            }
        }

    }

}
