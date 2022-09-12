using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using PurchasingSystem.DAL;
using PurchasingSystem.Models;
using PurchasingSystem.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
#endregion

namespace PurchasingSystem.BLL
{
    public class PurchasingServices
    {
        #region Constructor and Context Dependency
        private readonly PurchasingDbContext _context;

        internal PurchasingServices(PurchasingDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        public PurchaseOrderInfo GetPurchaseOrder(int vendorID)
        {
            if (vendorID < 0) throw new ArgumentNullException("Vendor ID was not supplied.");

            PurchaseOrderInfo? purchaseOrderExists = _context.PurchaseOrders
                                                             .Where(po => po.VendorID == vendorID
                                                                       && po.OrderDate == null)
                                                             .Select(po => new PurchaseOrderInfo
                                                             {
                                                                 PurchaseOrderID = po.PurchaseOrderID,
                                                                 PurchaseOrderNumber = po.PurchaseOrderNumber
                                                             })
                                                             .FirstOrDefault();
            return purchaseOrderExists;
        }

        public List<PurchaseOrderItem> GetSuggestedOrder(int vendorID)
        {
            if (vendorID < 0) throw new ArgumentNullException("No Vendor ID was supplied. Please try again.");

            List<PurchaseOrderItem> suggestedOrder = _context.Parts
                                                             .Where(p => p.VendorID == vendorID
                                                                      && p.ReorderLevel - (p.QuantityOnHand + p.QuantityOnOrder) > 0)
                                                             .Select(p => new PurchaseOrderItem
                                                             {
                                                                PartID = p.PartID,
                                                                Description = p.Description,
                                                                PurchasePrice = p.PurchasePrice,
                                                                ReorderLevel = p.ReorderLevel,
                                                                QuantityOnHand = p.QuantityOnHand,
                                                                QuantityOnOrder = p.QuantityOnOrder,
                                                                QuantityToOrder = p.ReorderLevel - (p.QuantityOnHand + p.QuantityOnOrder)
                                                             })
                                                             .OrderBy(p => p.PartID)
                                                             .ToList();
            return suggestedOrder;
        }

        public List<PurchaseOrderItem> GetActiveOrder(int purchaseOrderID)
        {
            if (purchaseOrderID < 0) throw new ArgumentNullException("No Purchase Order ID was supplied. Please try again.");

            List<PurchaseOrderItem> activeOrder = _context.PurchaseOrderDetails
                                                          .Where(pod => pod.PurchaseOrderID == purchaseOrderID)
                                                          .Select(pod => new PurchaseOrderItem
                                                          {
                                                              PurchaseOrderDetailID = pod.PurchaseOrderDetailID,
                                                              PartID = pod.PartID,
                                                              Description = pod.Part.Description,
                                                              PurchasePrice = pod.PurchasePrice,
                                                              QuantityOnHand = pod.Part.QuantityOnHand,
                                                              ReorderLevel = pod.Part.ReorderLevel,
                                                              QuantityOnOrder = pod.Part.QuantityOnOrder,
                                                              QuantityToOrder = pod.Quantity
                                                          })
                                                          .OrderBy(pod => pod.PartID)
                                                          .ToList();
            return activeOrder;
        }
        #endregion

        #region Commands
        public void CreateNewOrder(int vendorID, int employeeID, List<PurchaseOrderItem> currentOrder)
        {
            #region Global Variables
            const decimal GST = 0.05m;

            List<Exception> errorList = new();
            decimal subtotal = 0.00m,
                    gst = 0.00m;
            #endregion

            #region Parameter Validation
            if (vendorID < 0) throw new ArgumentNullException("No Vendor ID was supplied. Please try again.");
            if (employeeID < 0) throw new ArgumentNullException("No EmployeeID was supplied. Please try again.");
            if (currentOrder == null) throw new ArgumentNullException("No list of purchase order items was supplied. Please try again.");
            #endregion

            Vendor? vendorExists = _context.Vendors.Where(v => v.VendorID == vendorID).FirstOrDefault();

            if(vendorExists == null)
            {
                errorList.Add(new NullReferenceException($"There is no record of Vendor ({vendorID}) in our system."));
            }

            Employee? employeeExists = _context.Employees.Where(e => e.EmployeeID == employeeID).FirstOrDefault();

            if(employeeExists == null)
            {
                errorList.Add(new NullReferenceException($"There is no record of Employee ({employeeID}) in our system."));
            }

            List<PurchaseOrder> purchaseOrders = _context.PurchaseOrders.ToList();

            foreach (var item in currentOrder)
            {
                Part? partExists = _context.Parts.Where(p => p.PartID == item.PartID).FirstOrDefault();

                if(partExists == null)
                {
                    errorList.Add(new NullReferenceException($"There is no record of Part ({item.PartID}) in our system."));
                }

                subtotal += item.PurchasePrice * (decimal)item.QuantityToOrder;
                gst += (item.PurchasePrice * GST) * (decimal)item.QuantityToOrder;
            }

            PurchaseOrder newOrder = new PurchaseOrder();

            newOrder.PurchaseOrderNumber = purchaseOrders.Max(x => x.PurchaseOrderNumber) + 1;
            newOrder.VendorID = vendorID;
            newOrder.EmployeeID = employeeID;
            newOrder.SubTotal = subtotal;
            newOrder.TaxAmount = gst;

            vendorExists.PurchaseOrders.Add(newOrder);

            foreach(var item in currentOrder)
            {
                PurchaseOrderDetail newOrderDetail = new PurchaseOrderDetail();

                newOrderDetail.PartID = item.PartID;
                newOrderDetail.Quantity = (int)item.QuantityToOrder;
                newOrderDetail.PurchasePrice = item.PurchasePrice;
                newOrderDetail.PurchaseOrderID = newOrder.PurchaseOrderID;

                newOrder.PurchaseOrderDetails.Add(newOrderDetail);
            }

            if(errorList.Count > 0)
                throw new AggregateException("Unable to process your request.", errorList);
            else
                _context.SaveChanges();
        }

        public void SaveActiveOrder(int employeeID, PurchaseOrderInfo purchaseOrder, List<PurchaseOrderItem> currentOrder)
        {
            #region Global Variables
            const decimal GST = 0.05m;

            decimal subtotal = 0.00m,
                    gst = 0.00m;

            List<Exception> errorList = new();
            List<PurchaseOrderDetail> orderItemsToRemove = new();
            #endregion

            #region Parameter Validation
            if (purchaseOrder == null) throw new ArgumentNullException("Purchase order is required. Please try again.");
            if (purchaseOrder.PurchaseOrderID == 0) throw new ArgumentNullException("Purchase Order ID is missing. Please try again.");
            if (employeeID <= 0) throw new ArgumentNullException("Employee ID is required. Please try again.");
            if (currentOrder == null) throw new ArgumentNullException("List of purchase order items is required. Please try again.");
            #endregion

            PurchaseOrder? purchaseOrderExists = _context.PurchaseOrders
                                                         .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                                         .FirstOrDefault();
            if(purchaseOrderExists == null)
                throw new ArgumentException($"Purchase Order ({purchaseOrder.PurchaseOrderNumber}) does not exist. Please try again.");

            Employee? employeeExists = _context.Employees
                                              .Where(p => p.EmployeeID == employeeID)
                                              .FirstOrDefault();

            if (employeeExists == null)
                throw new ArgumentException($"Employee ({employeeID}) does not exist. Please try a different user.");

            List<PurchaseOrderDetail> purchaseOrderItems = _context.PurchaseOrderDetails
                                                                     .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                                                     .ToList();

            foreach(var purchaseOrderItem in purchaseOrderItems)
            {
                if(!currentOrder.Any(oi => oi.PurchaseOrderDetailID == purchaseOrderItem.PurchaseOrderDetailID))
                {
                    orderItemsToRemove.Add(purchaseOrderItem);
                }
            }

            if(orderItemsToRemove.Count > 0)
            {
                foreach(var orderItem in orderItemsToRemove)
                {
                    _context.PurchaseOrderDetails.Remove(orderItem);
                }
            }

            foreach (var item in currentOrder)
            {
                errorList = ValidateOrderItem(item, errorList);

                subtotal += (decimal)item.QuantityToOrder * item.PurchasePrice;
                gst += (item.PurchasePrice * GST) * (decimal)item.QuantityToOrder;

                PurchaseOrderDetail? orderDetailExists = _context.PurchaseOrderDetails
                                                                 .Where(pod => pod.PurchaseOrderDetailID == item.PurchaseOrderDetailID)
                                                                 .FirstOrDefault();

                if(orderDetailExists == null)
                {
                    PurchaseOrderDetail newOrderDetail = new PurchaseOrderDetail
                    {
                        PurchaseOrderID = (int)purchaseOrder.PurchaseOrderID,
                        PartID = item.PartID,
                        Quantity = (int)item.QuantityToOrder,
                        PurchasePrice = item.PurchasePrice,
                    };

                    purchaseOrderExists.PurchaseOrderDetails.Add(newOrderDetail);
                }
                else
                {
                    orderDetailExists.Quantity = (int)item.QuantityToOrder;
                    orderDetailExists.PurchasePrice = item.PurchasePrice;
                    EntityEntry<PurchaseOrderDetail> updatedOrderDetail = _context.Entry(orderDetailExists);
                    updatedOrderDetail.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }
            }

            purchaseOrderExists.SubTotal = subtotal;
            purchaseOrderExists.TaxAmount = gst;
            purchaseOrderExists.EmployeeID = employeeID;

            EntityEntry<PurchaseOrder> updatedPurchaseOrder = _context.Entry(purchaseOrderExists);
            updatedPurchaseOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            if (errorList.Count > 0)
                throw new AggregateException("Unable to update the current order.", errorList);
            else
                _context.SaveChanges();
        }

        public void PlaceActiveOrder(int employeeID, PurchaseOrderInfo purchaseOrder, List<PurchaseOrderItem> currentOrder)
        { 
            #region Global Variables
            const decimal GST = 0.05m;

            decimal subtotal = 0.00m,
                    gst = 0.00m;

            List<Exception> errorList = new();
            List<PurchaseOrderDetail> orderItemsToRemove = new();
            #endregion

            if (employeeID <= 0)
                throw new ArgumentNullException("Employee ID is required. Please try again.");
            if (purchaseOrder == null)
                throw new ArgumentNullException("We could not find an instance of the purchase order. Please try again.");
            if (currentOrder.Count <= 0)
                throw new ArgumentNullException("There are no items on the order. Please try again.");

            Employee? employeeExists = _context.Employees
                                              .Where(x => x.EmployeeID == employeeID).FirstOrDefault();
            if (employeeExists == null)
                throw new ArgumentException($"Employee ({employeeID}) does not exist. Please try another user.");

            PurchaseOrder? purchaseOrderExists = _context.PurchaseOrders
                                                        .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                                        .FirstOrDefault();
            if (purchaseOrderExists == null)
                throw new ArgumentException($"Purchase Order ({purchaseOrder.PurchaseOrderNumber}) does not exist. Please try again.");

            List<PurchaseOrderDetail> purchaseOrderItems = _context.PurchaseOrderDetails
                                                                     .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                                                     .ToList();

            foreach (var purchaseOrderItem in purchaseOrderItems)
            {
                if (!currentOrder.Any(oi => oi.PurchaseOrderDetailID == purchaseOrderItem.PurchaseOrderDetailID))
                {
                    orderItemsToRemove.Add(purchaseOrderItem);
                }
            }

            if (orderItemsToRemove.Count > 0)
            {
                foreach (var orderItem in orderItemsToRemove)
                {
                    _context.PurchaseOrderDetails.Remove(orderItem);
                }
            }

            foreach (var item in currentOrder)
            {
                errorList = ValidateOrderItem(item, errorList);

                subtotal += (decimal)item.QuantityToOrder * item.PurchasePrice;
                gst += (item.PurchasePrice * GST) * (decimal)item.QuantityToOrder;

                PurchaseOrderDetail? orderDetailExists = _context.PurchaseOrderDetails
                                                                 .Where(pod => pod.PurchaseOrderDetailID == item.PurchaseOrderDetailID)
                                                                 .FirstOrDefault();

                if (orderDetailExists == null)
                {
                    PurchaseOrderDetail newOrderDetail = new PurchaseOrderDetail
                    {
                        PurchaseOrderID = (int)purchaseOrder.PurchaseOrderID,
                        PartID = item.PartID,
                        Quantity = (int)item.QuantityToOrder,
                        PurchasePrice = item.PurchasePrice,
                    };

                    purchaseOrderExists.PurchaseOrderDetails.Add(newOrderDetail);
                }
                else
                {
                    orderDetailExists.Quantity = (int)item.QuantityToOrder;
                    orderDetailExists.PurchasePrice = item.PurchasePrice;
                    EntityEntry<PurchaseOrderDetail> updatedOrderDetail = _context.Entry(orderDetailExists);
                    updatedOrderDetail.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                }

                Part? part = _context.Parts.Where(p => p.PartID == item.PartID).FirstOrDefault();

                part.QuantityOnOrder += (int)item.QuantityToOrder;
                EntityEntry updatedPart = _context.Entry(part);
                updatedPart.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            }

            purchaseOrderExists.SubTotal = subtotal;
            purchaseOrderExists.TaxAmount = gst;
            purchaseOrderExists.EmployeeID = employeeID;
            purchaseOrderExists.OrderDate = DateTime.Now;

            EntityEntry<PurchaseOrder> updatedPurchaseOrder = _context.Entry(purchaseOrderExists);
            updatedPurchaseOrder.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            if (errorList.Count > 0)
                throw new AggregateException("Unable to update the current order.", errorList);
            else
                _context.SaveChanges();
        }

        public void DeleteActiveOrder(PurchaseOrderInfo purchaseOrder)
        {
            #region Global Variables
            List<PurchaseOrderDetail> orderDetails = new();
            #endregion

            if (purchaseOrder == null)
                throw new ArgumentNullException("No Purchase Order specified. Please try again.");
            if (purchaseOrder.PurchaseOrderID <= 0)
                throw new ArgumentNullException("No Purchase Order specified. Please try again.");

            PurchaseOrder? purchaseOrderExists = _context.PurchaseOrders
                                                         .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                                         .FirstOrDefault();
            if (purchaseOrderExists == null)
                throw new ArgumentException("The specified Purchase Order does not exist. Please try again.");

            orderDetails = _context.PurchaseOrderDetails
                                   .Where(p => p.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                   .ToList();

            foreach(PurchaseOrderDetail detail in orderDetails)
            {
                _context.PurchaseOrderDetails.Remove(detail);
            }

            _context.PurchaseOrders.Remove(purchaseOrderExists);
            _context.SaveChanges();
        }
        #endregion

        #region Helper Methods
        public List<Exception> ValidateOrderItem(PurchaseOrderItem item, List<Exception> errorList)
        {
            if (item.PartID <= 0)
                errorList.Add(new Exception("PartID is missing. Please try again."));

            Part? partExists = _context.Parts.Where(p => p.PartID == item.PartID).FirstOrDefault();

            if (partExists == null)
                errorList.Add(new Exception($"We could not find a record for Part ({item.PartID}). Please try again."));
            if (item.PurchasePrice <= 0)
                errorList.Add(new Exception($"Purchase price for part {item.PartID} must be greater than 0. Please try again."));
            if (item.QuantityToOrder == 0)
                errorList.Add(new Exception("Quanity To Order must be greater than 0."));
            if (item.QuantityOnHand < 0)
                errorList.Add(new Exception("Quantity on Hand can not be negative."));
            if (item.QuantityOnOrder < 0)
                errorList.Add(new Exception("Quantity on Order can not be negative."));
            if (item.ReorderLevel < 0)
                errorList.Add(new Exception("Reorder Level must be greater than 0"));

            return errorList;
        }
        #endregion
    }
}