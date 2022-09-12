using Microsoft.EntityFrameworkCore.ChangeTracking;
using RecievingSystem.DAL;
using RecievingSystem.Entities;
using RecievingSystem.ViewModels;
/// <summary>
/// Create AID to 
/// </summary>
namespace RecievingSystem.BLL
{

    public class RecievingOrderDetailsServices
    {
        private readonly RecievingDbContext _context;

        internal RecievingOrderDetailsServices(RecievingDbContext context)
        {
            _context = context;
        }
        #region Purchase Order Detail Queries
        public List<OutStandingOrderDetail> FetchOutStandingOrderDetailsByID(int purchaseOrderID)
        {
            List<Exception> brokenRules = new List<Exception>();
            bool orderExistsAndOpen = _context.PurchaseOrders.Where(x => x.Closed == false).Where(x => x.PurchaseOrderID == purchaseOrderID).Count() <= 1;
            if (!orderExistsAndOpen)
            {
                brokenRules.Add(new NullReferenceException("There is not a open purchase Order with that ID"));
            }

            var ordersQuery = _context.PurchaseOrderDetails.Where(x => x.PurchaseOrderID == purchaseOrderID)
                .Select(x => new OutStandingOrderDetail
                {
                    PurcahseOrderID = x.PurchaseOrderID,
                    PurchaseOrderDetailID = x.PurchaseOrderDetailID,
                    PartID = x.PartID,
                    PartDescription =
                    _context.Parts.Where(y => y.PartID == x.PartID)
                    .Where(y => y.Description != null)
                    .Select(z => z.Description)
                    .FirstOrDefault() ?? "",
                    OustandingQtyOriginal = x.Quantity,
                    RecieveOrderDetailID = _context.ReceiveOrderDetails
                        .Where(y => y.PurchaseOrderDetailID == purchaseOrderID)
                        .Where(y => y.ReceiveOrderDetailID != null)
                        .Select(a => a.ReceiveOrderDetailID).FirstOrDefault(),
                    OutStandingQty =
                      x.Quantity - _context.ReceiveOrderDetails
                        .Where(y => y.PurchaseOrderDetailID == x.PurchaseOrderDetailID)
                        .Where(a => a.QuantityReceived != null)//For some reason, since it is weird linq int, I can't just add ?? operator, so I used .Where()
                        .Select(z => z.QuantityReceived).Sum(),
                }).ToList();
            return ordersQuery;
        }
        public OutStandingOrder FetchOutStandingOrder(int id)
        {
            var ordersQuery = _context.PurchaseOrders.Where(x => x.Closed == false).Where(x => x.PurchaseOrderID == id).Select(x => new OutStandingOrder
            {
                PurchaseOrderID = x.PurchaseOrderID,
                OrderDate = x.OrderDate,
                VendorID = x.VendorID,
                VendorName = _context.Vendors.Where(y => y.VendorID == x.VendorID)
                    .Select(z => z.VendorName)
                    .FirstOrDefault() ?? "",

                PhoneNumber = _context.Vendors.Where(y => y.VendorID == x.VendorID)
                    .Select(z => z.Phone)
                    .FirstOrDefault() ?? "N/A"

            }).FirstOrDefault();
            return ordersQuery;
        }
        #endregion
        #region Recieve and ForceClose
        public void RecieveOrder(int purchaseOrderID, int employeeID, List<RecievedItems> items)
        {
            List<Exception> brokenRules = new List<Exception>();
            Console.WriteLine("recieveOrder Messages: ");
            bool canCloseOrder = true;
            bool isReturnables = false;
            bool isRecievables = false;
            bool noErrors = false;
            List<RecievedItems> returnables = new();
            List<RecievedItems> recievables = new();
            foreach (var item in items)
            {
                if (item.QuantityRecieved != item.QuantityOutStanding)
                {
                    canCloseOrder = false;
                }

                if (item.QuantityReturned > 0)
                {
                    isReturnables = true;
                    if (item.ReturnReason == "" || item.ReturnReason == null)
                    {
                        brokenRules.Add(new ArgumentException($"Must have a return reason for {item.PartDescription} if you're returning a Quantity!"));
                    }
                }
                if (item.QuantityRecieved > item.QuantityOutStanding)
                {
                    brokenRules.Add(new ArgumentException($"Cannot Recieve more  of {item.PartDescription} then items needed! Return the rest of the items "));
                }
                //if (item.QuantityReturned == 0 && item.QuantityRecieved == 0)
                //{
                //    brokenRules.Add(new ArgumentException($"Quantity returned and Quantity recieved cannot both be 0 for {item.PartDescription}"));
                //}
                if (item.QuantityReturned > 0)
                {
                    returnables.Add(item);
                }
                if (item.QuantityRecieved > 0)
                {
                    recievables.Add(item);
                    isRecievables = true;
                }
            }
            if (returnables.Count() == 0 && recievables.Count() == 0)
            {
                brokenRules.Add(new Exception("Need at least 1 item that is recieved or can be returned!"));
            }

            //canCloseOrder = returnables.Count() == 0;
            Console.WriteLine($" Can close Order: {canCloseOrder} ");
            Console.WriteLine($" are there returnables: {isReturnables} ");

            var recieveOrderEntries = _context.ReceiveOrders.Where(a => a.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
            if (recieveOrderEntries == null)
            {
                try
                {
                    ReceiveOrder newRecieved = new ReceiveOrder();
                    newRecieved.PurchaseOrderID = purchaseOrderID;
                    newRecieved.EmployeeID = employeeID;
                    newRecieved.ReceiveDate = DateTime.Now;

                    EntityEntry<ReceiveOrder> addNewRecieving = _context.Entry(newRecieved);
                    addNewRecieving.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    _context.SaveChanges(); // is it best practices to save changes here? because I need a recieving order entry if there is none....
                    recieveOrderEntries = _context.ReceiveOrders.Where(a => a.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
                    Console.WriteLine($"Recieved Order:  Purchase Order ID: {recieveOrderEntries.PurchaseOrderID} Recieve Order ID: {recieveOrderEntries.ReceiveOrderID}");
                }
                catch (Exception ex)
                {
                    brokenRules.Add(new Exception("Error: Could not create new RecieveOrder entry!"));
                }
            }
            noErrors = brokenRules.Count() == 0;
            Console.WriteLine($"Recieved Order:  Purchase Order ID: {recieveOrderEntries.PurchaseOrderID} Recieve Order ID: {recieveOrderEntries.ReceiveOrderID}");
            if (!noErrors)
            {
                throw new AggregateException(brokenRules);
            }
            if (noErrors)
            {
                try
                {
                    if (isReturnables)
                    {
                        foreach (var item in returnables)
                        {
                            ReturnedOrderDetail returnable = new ReturnedOrderDetail();
                            returnable.PurchaseOrderDetailID = item.PurchaseOrderDetailID;
                            returnable.ReceiveOrderID = recieveOrderEntries.ReceiveOrderID;
                            returnable.Reason = item.ReturnReason ?? "";
                            returnable.Quantity = item.QuantityReturned;
                            returnable.ItemDescription = item.PartDescription;

                            EntityEntry<ReturnedOrderDetail> entityEntry = _context.Entry(returnable);
                            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        }
                    }
                    if (isRecievables)
                    {
                        foreach (var item in recievables)
                        {
                            ReceiveOrderDetail receiveOrderDetail = new ReceiveOrderDetail();
                            receiveOrderDetail.QuantityReceived = item.QuantityRecieved;
                            receiveOrderDetail.ReceiveOrderID = recieveOrderEntries.ReceiveOrderID;
                            recieveOrderEntries.ReceiveDate = DateTime.Now;
                            receiveOrderDetail.PurchaseOrderDetailID = item.PurchaseOrderDetailID;

                            EntityEntry<ReceiveOrderDetail> entityEntryReceiving = _context.Entry(receiveOrderDetail);
                            entityEntryReceiving.State = Microsoft.EntityFrameworkCore.EntityState.Added;

                            Part part = _context.Parts.Where(x => x.PartID == item.PartID).FirstOrDefault();
                            part.QuantityOnHand += item.QuantityRecieved;
                            EntityEntry<Part> entityEntryPart = _context.Entry(part);
                            entityEntryPart.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                        }
                    }
                    PurchaseOrder purchaseOrder = _context.PurchaseOrders.Where(x => x.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
                    purchaseOrder.Closed = canCloseOrder;
                    EntityEntry<PurchaseOrder> entityPurchaseOrder = _context.Entry(purchaseOrder);
                    entityPurchaseOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving/Updating to DB");

                }

            }
            Console.WriteLine(" ******************************ERRORS*****************************************************");
            foreach (var error in brokenRules)
            {
                Console.WriteLine(error.Message);
            }
        }
        public void ForceRecieveOrder(int purchaseOrderID, int employeeID, List<RecievedItems> items, string reason)
        {
            List<Exception> brokenRules = new List<Exception>();
            Console.WriteLine("recieveOrder Messages: ");
            bool canCloseOrder = true;
            bool isReturnables = false;
            bool isRecievables = false;
            bool noErrors = false;
            List<RecievedItems> returnables = new List<RecievedItems>();
            List<RecievedItems> recievables = new List<RecievedItems>();
            if (string.IsNullOrEmpty(reason))
            {
                brokenRules.Add(new Exception($"To close order PO#{purchaseOrderID}, you must provide a reason to close"));
            }
            foreach (var item in items)
            {

                if (item.QuantityReturned > 0)
                {
                    isReturnables = true;
                    if (item.ReturnReason == "" || item.ReturnReason == null)
                    {
                        brokenRules.Add(new ArgumentException($"Must have a return reason for {item.PartDescription} if you're returning a Quantity!"));
                    }
                }
                if (item.QuantityRecieved > item.QuantityOutStanding)
                {
                    brokenRules.Add(new ArgumentException($"Cannot Recieve more  of {item.PartDescription} then items needed! Return the rest of the items "));
                }

                if (item.QuantityReturned > 0)
                {
                    returnables.Add(item);
                }
                if (item.QuantityRecieved > 0)
                {
                    recievables.Add(item);
                    isRecievables = true;
                }
            }
            noErrors = brokenRules.Count() == 0;

            Console.WriteLine($" are there returnables: {isReturnables} ");

            var recieveOrderEntries = _context.ReceiveOrders.Where(a => a.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
            if (recieveOrderEntries == null)
            {
                try
                {
                    ReceiveOrder newRecieved = new ReceiveOrder();
                    newRecieved.PurchaseOrderID = purchaseOrderID;
                    newRecieved.EmployeeID = employeeID;
                    newRecieved.ReceiveDate = DateTime.Now;

                    EntityEntry<ReceiveOrder> addNewRecieving = _context.Entry(newRecieved);
                    addNewRecieving.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    _context.SaveChanges(); // is it best practices to save changes here? because I need a recieving order entry if there is none....
                    recieveOrderEntries = _context.ReceiveOrders.Where(a => a.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
                    Console.WriteLine($"Recieved Order:  Purchase Order ID: {recieveOrderEntries.PurchaseOrderID} Recieve Order ID: {recieveOrderEntries.ReceiveOrderID}");
                }
                catch (Exception ex)
                {
                    brokenRules.Add(new Exception("Error: Could not create new RecieveOrder entry!"));
                }
            }
            noErrors = brokenRules.Count() == 0;
            Console.WriteLine($"Recieved Order:  Purchase Order ID: {recieveOrderEntries.PurchaseOrderID} Recieve Order ID: {recieveOrderEntries.ReceiveOrderID}");
            if (!noErrors)
            {
                throw new AggregateException(brokenRules);
            }
            if (noErrors)
            {
                try
                {
                    if (isReturnables)
                    {
                        foreach (var item in returnables)
                        {
                            ReturnedOrderDetail returnable = new ReturnedOrderDetail();
                            returnable.PurchaseOrderDetailID = item.PurchaseOrderDetailID;
                            returnable.ReceiveOrderID = recieveOrderEntries.ReceiveOrderID;
                            returnable.Reason = item.ReturnReason ?? "";
                            returnable.Quantity = item.QuantityReturned;
                            returnable.ItemDescription = item.PartDescription;

                            EntityEntry<ReturnedOrderDetail> entityEntry = _context.Entry(returnable);
                            entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                        }


                    }
                    if (isRecievables)
                    {
                        foreach (var item in recievables)
                        {
                            ReceiveOrderDetail receiveOrderDetail = new ReceiveOrderDetail();
                            receiveOrderDetail.QuantityReceived = item.QuantityRecieved;
                            receiveOrderDetail.ReceiveOrderID = recieveOrderEntries.ReceiveOrderID;
                            recieveOrderEntries.ReceiveDate = DateTime.Now;
                            receiveOrderDetail.PurchaseOrderDetailID = item.PurchaseOrderDetailID;

                            EntityEntry<ReceiveOrderDetail> entityEntryReceiving = _context.Entry(receiveOrderDetail);
                            entityEntryReceiving.State = Microsoft.EntityFrameworkCore.EntityState.Added;

                            Part part = _context.Parts.Where(x => x.PartID == item.PartID).FirstOrDefault();
                            part.QuantityOnHand += item.QuantityRecieved;
                            EntityEntry<Part> entityEntryPart = _context.Entry(part);
                            entityEntryPart.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                        }
                    }
                    PurchaseOrder purchaseOrder = _context.PurchaseOrders.Where(x => x.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
                    purchaseOrder.Closed = true;
                    purchaseOrder.Notes = $" Force Closed: {reason}";
                    EntityEntry<PurchaseOrder> entityPurchaseOrder = _context.Entry(purchaseOrder);
                    entityPurchaseOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error saving/Updating to DB");

                }
            }
            Console.WriteLine(" ******************************ERRORS*****************************************************");
            foreach (var error in brokenRules)
            {
                Console.WriteLine(error.Message);
            }
        }


        #endregion
        #region UnOrdered Items Table
        public void InsertUnOrderedItem(UnOrderedItemToAdd item)
        {
            List<Exception> brokenRules = new List<Exception>();
            try
            {
                var query = _context.UnorderedPurchaseItemCarts.Where(x => x.VendorPartNumber == item.VendorPartNumber)
                        .Where(x => x.Description == item.Description);
                if (query.Count() > 0)
                {
                    brokenRules.Add(new Exception("There is already that exists with thhat Description and Vendor Part Number"));

                }
                if (string.IsNullOrEmpty(item.VendorPartNumber))
                {
                    brokenRules.Add(new Exception("Must have a Vendor Part Number for UnOrdered items"));
                }
                if (string.IsNullOrEmpty(item.Description))
                {
                    brokenRules.Add(new Exception("Must have a Description for UnOrdered items"));
                }
                if (item.Quantity <= 0)
                {
                    brokenRules.Add(new Exception("Quantity must be above 0 for UnOrdered items"));
                }
                if (brokenRules.Count == 0)
                {
                    UnorderedPurchaseItemCart newITem = new UnorderedPurchaseItemCart();
                    newITem.VendorPartNumber = item.VendorPartNumber;
                    newITem.Description = item.Description;
                    newITem.Quantity = item.Quantity;
                    EntityEntry<UnorderedPurchaseItemCart> cartEntry = _context.Entry(newITem);
                    cartEntry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    _context.SaveChanges();
                }
                else
                {
                    throw new AggregateException(brokenRules);
                }



            }


            catch (Exception ex)
            {
                throw new AggregateException(brokenRules);
            }
            foreach (var error in brokenRules)
            {
                Console.WriteLine(error.Message);
            }
        }
        public List<UnOrderedItem> FetchUnorderedItems()
        {

            var query = _context.UnorderedPurchaseItemCarts
                .Select(x => new UnOrderedItem
                {
                    Description = x.Description,
                    VendorPartNumber = x.VendorPartNumber,
                    CardID = x.CartID,
                    Quantity = x.Quantity
                }).ToList();
            return query;
        }
        public void deleteUnOrderedItem(int id)
        {
            var deleteItem = _context.UnorderedPurchaseItemCarts.Where(x => x.CartID == id).SingleOrDefault();

            EntityEntry<UnorderedPurchaseItemCart> cartEntry = _context.Entry(deleteItem);
            cartEntry.State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            _context.SaveChanges();
        }
        public void saveCartItems(List<UnOrderedItem> items, int purchaseOrderID, int employeeID)
        {
            List<Exception> brokenRules = new List<Exception>();
            var recieveOrderEntries = _context.ReceiveOrders.Where(a => a.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
            if (recieveOrderEntries == null)
            {
                try
                {
                    ReceiveOrder newRecieved = new ReceiveOrder();
                    newRecieved.PurchaseOrderID = purchaseOrderID;
                    newRecieved.EmployeeID = employeeID;
                    newRecieved.ReceiveDate = DateTime.Now;

                    EntityEntry<ReceiveOrder> addNewRecieving = _context.Entry(newRecieved);
                    addNewRecieving.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                    _context.SaveChanges(); // is it best practices to save changes here? because I need a recieving order entry if there is none....
                    recieveOrderEntries = _context.ReceiveOrders.Where(a => a.PurchaseOrderID == purchaseOrderID).FirstOrDefault();
                    Console.WriteLine($"Recieved Order:  Purchase Order ID: {recieveOrderEntries.PurchaseOrderID} Recieve Order ID: {recieveOrderEntries.ReceiveOrderID}");

                }
                catch (Exception ex)
                {
                    brokenRules.Add(new Exception("Error: Could not create new RecieveOrder entry!"));
                }
            }
            foreach (var item in items)
            {
                ReturnedOrderDetail returnable = new ReturnedOrderDetail();
                returnable.ReceiveOrderID = recieveOrderEntries.ReceiveOrderID;
                returnable.Reason = "Not Ordered!";
                returnable.Quantity = item.Quantity;
                returnable.ItemDescription = item.Description;
                returnable.VendorPartNumber = item.VendorPartNumber;

                EntityEntry<ReturnedOrderDetail> entityEntry = _context.Entry(returnable);
                entityEntry.State = Microsoft.EntityFrameworkCore.EntityState.Added;
                Console.WriteLine("ITEM DID STUFF");
            }
            _context.SaveChanges();
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
    #endregion
}


