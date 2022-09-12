using SalesSystem.DAL;
using SalesSystem.Entities;
using SalesSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.BLL
{
    public class PartServices
    {
        #region Constructor and Context Dependency
        private readonly SalesDbContext _context;
        internal PartServices(SalesDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        // Get Part information for form
        public List<PartsListBy> PartInfo(int partid)
        {
            List<PartsListBy> info = _context.Parts
                                            .Where(x => x.PartID == partid)
                                            .Select(x => new PartsListBy
                                            {
                                                PartID = x.PartID,
                                                Description = x.Description,
                                                SellingPrice = x.SellingPrice
                                            })
                                            .ToList();
            return info;
        }

        public List<SelectionList> GetPartsInCategory(int categoryid)
        {
            List<SelectionList> info = _context.Parts
                                        .Where(x => x.CategoryID == categoryid)
                                        .Select(x => new SelectionList
                                        {
                                            ValueID = x.PartID,
                                            DisplayText = x.Description
                                        })
                                        .ToList();
            return info;
        }
        #endregion

        #region Commands
        // For when the user clicks "Checkout"
        public void CreateSale(int saleID, int employeeID, string paymentType, DateTime saleDate,  int? couponID, List<PartsListBy> currentCart)
        {
            decimal GST = 0.05m;
            decimal subtotal = 0.00m;
            decimal gst = 0.00m;

            List<Exception> brokenRules = new();

            // Make sure employee exists
            Employee? employeeExists = _context.Employees.Where(x => x.EmployeeID == employeeID).FirstOrDefault();
            if (employeeExists == null)
            {
                brokenRules.Add(new NullReferenceException("Employee does not exist"));
            }

            List<Sale> sales = _context.Sales.ToList();
            List<SaleDetail> salesDetail = _context.SaleDetails.ToList();

            // Calculate the total and the subtotal
            foreach (var item in currentCart)
            {
                Part? partExists = _context.Parts.Where(x => x.PartID == item.PartID).FirstOrDefault();

                subtotal += item.SellingPrice * item.QuantityOnOrder;
                gst += (item.SellingPrice * GST) * item.QuantityOnOrder;
            }

            // Add in the new sale
            Sale newSale = new Sale();
            newSale.SaleID = saleID;
            newSale.EmployeeID = employeeID;
            newSale.SubTotal = subtotal;
            newSale.TaxAmount = gst;
            newSale.PaymentType = paymentType;
            newSale.SaleDate = saleDate;
            newSale.CouponID = couponID;

            sales.Add(newSale);

            // Add in the new sale details
            foreach(var item in currentCart)
            {
                SaleDetail newSaleDetail = new SaleDetail();

                newSaleDetail.SaleDetailID = salesDetail.Max(x => x.SaleDetailID) + 1;
                newSaleDetail.PartID = item.PartID;
                newSaleDetail.Quantity = item.QuantityOnOrder;
                newSaleDetail.SellingPrice = item.SellingPrice;

                newSale.SaleDetails.Add(newSaleDetail);
            }
        }

        public void RefundSale(int saleID)
        {

        }
        #endregion
    }
}
