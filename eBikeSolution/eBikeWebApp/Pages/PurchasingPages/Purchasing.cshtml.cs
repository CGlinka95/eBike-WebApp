using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional Namespaces
using eBikeWebApp.Data;
using Microsoft.AspNetCore.Identity;
using AppSecurity.BLL;
using PurchasingSystem.Models;
using PurchasingSystem.BLL;
using WebApp.Helpers;
#endregion

namespace eBikeWebApp.Pages.PurchasingPages
{
    public class PurchasingModel : PageModel
    {
        #region Constructor and Dependency Injection
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SecurityService _Security;
        private readonly PurchasingServices _purchasingServices;
        private readonly VendorServices _vendorServices;

        public PurchasingModel(UserManager<ApplicationUser> userManager, SecurityService security, PurchasingServices purchasingServices, VendorServices vendorServices)
        {
            _UserManager = userManager;
            _Security = security;
            _purchasingServices = purchasingServices;
            _vendorServices = vendorServices;
        }
        #endregion

        #region Security
        public ApplicationUser AppUser { get; set; }
        public string EmployeeName { get; set; }
        #endregion

        #region Error Handling Properties
        [TempData]
        public string FeedbackMessage { get; set; }
        
        [TempData]
        public string ErrorMessage { get; set; }
        public bool HasFeedBack => !string.IsNullOrWhiteSpace(FeedbackMessage);
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public List<string> ErrorDetails { get; set; } = new();
        public List<Exception> Errors { get; set; } = new();
        #endregion

        #region Routing Parameters
        [BindProperty(SupportsGet = true)]
        public int? EmployeeID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VendorID { get; set; }
        #endregion

        #region Purchase Order Totals
        public decimal subtotal { get; set; }
        public decimal gst { get; set; }
        public decimal total { get; set; }
        #endregion

        private int threadTimeout = 500;

        [BindProperty]
        public int? partToMove { get; set; }

        [BindProperty]
        public int? vendorPartToMove { get; set; }
        public VendorInfo? Vendor { get; set; }
        public List<SelectionList> VendorList { get; set; } = new();

        [BindProperty]
        public List<PurchaseOrderItem> CurrentOrder { get; set; } = new();

        [BindProperty]
        public List<PurchaseOrderItem> OrderItems { get; set; } = new();

        [BindProperty]
        public PurchaseOrderInfo? PurchaseOrder { get; set; }

        [BindProperty]
        public List<PurchaseOrderItem> VendorInventory { get; set; }

        public void OnGet()
        {
            _ = GetActiveEmployee();
            Thread.Sleep(threadTimeout);
            PopulateVendorList();
            if(VendorID.HasValue && VendorID > 0)
            {
                // Get the Vendor by Vendor ID
                GetVendorInfo();

                // Get Purchase Order by VendorID
                GetPurchaseOrder();

                // Populate the CurrentOrder and VendoryInventory lists
                if (PurchaseOrder == null)
                {
                    CurrentOrder = _purchasingServices.GetSuggestedOrder((int)VendorID);
                }
                else
                {
                    CurrentOrder = _purchasingServices.GetActiveOrder((int)PurchaseOrder.PurchaseOrderID);
                }

                if(CurrentOrder != null)
                {
                    GetOrderTotals();
                }

                if(VendorInventory == null && CurrentOrder != null)
                { 
                    VendorInventory = _vendorServices.GetVendorInventory((int)VendorID, CurrentOrder);
                }
            }
        }

        public IActionResult OnPostFindOrder()
        {
            _ = GetActiveEmployee();
            Thread.Sleep(threadTimeout);
            PopulateVendorList();

            return RedirectToPage(new
            {
                EmployeeID = EmployeeID,
                VendorID = VendorID
            });
        }
        public IActionResult OnPostNewOrder()
        {
            try
            {
                if(VendorID <= 0)
                    Errors.Add(new Exception("You must select a vendor."));

                if(EmployeeID <= 0)
                    Errors.Add(new Exception("You must be logged in to perform this action."));

                if (CurrentOrder == null || CurrentOrder.Count <= 0)
                    Errors.Add(new Exception("You must select one item to purchase."));

                if (Errors.Any())
                    throw new AggregateException(Errors);

                _purchasingServices.CreateNewOrder((int)VendorID, (int)EmployeeID, CurrentOrder);

                FeedbackMessage = $"New order has been created.";

                return RedirectToPage(new
                {
                    EmployeeID = EmployeeID,
                    VendorID = VendorID
                });
            }
            catch(AggregateException ex)
            {
                ErrorMessage = "Unable to process your request.";
                foreach(var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                RepopulateFields();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                RepopulateFields();
                return Page();
            }
        }
        public IActionResult OnPostSaveOrder()
        {
            try
            {
                if (!EmployeeID.HasValue)
                    Errors.Add(new Exception("Employee ID is missing. Please try again."));

                if (PurchaseOrder == null)
                    Errors.Add(new Exception("There was an error with the purchase order. Please try again."));

                if (CurrentOrder.Count == 0)
                    Errors.Add(new Exception("There was an error with the current order list. Please try again."));

                if (Errors.Any())
                    throw new AggregateException(Errors);

                _purchasingServices.SaveActiveOrder((int)EmployeeID, PurchaseOrder, CurrentOrder);

                FeedbackMessage = $"Order {PurchaseOrder.PurchaseOrderNumber} has been updated.";

                return RedirectToPage(new
                {
                    EmployeeID = EmployeeID,
                    VendorID = VendorID
                });
            }
            catch(AggregateException ex)
            {
                ErrorMessage = "Unable to process your request.";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                RepopulateFields();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                RepopulateFields();
                return Page();
            }
        }
        public IActionResult OnPostPlaceOrder()
        {
            try
            {
                if (!EmployeeID.HasValue)
                    Errors.Add(new Exception("Employee ID is missing. Please try again."));

                if (PurchaseOrder == null)
                    Errors.Add(new Exception("There was an error with the purchase order. Please try again."));

                if (CurrentOrder.Count == 0)
                    Errors.Add(new Exception("There was an error with the current order list. Please try again."));

                if (Errors.Any())
                    throw new AggregateException(Errors);

                _purchasingServices.PlaceActiveOrder((int)EmployeeID, PurchaseOrder, CurrentOrder);

                FeedbackMessage = $"Order ({PurchaseOrder.PurchaseOrderNumber}) placed.";

                return RedirectToPage(new
                {
                    EmployeeID = EmployeeID,
                    VendorID = VendorID
                });
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process your request.";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                RepopulateFields();
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                RepopulateFields();
                return Page();
            }
        }
        public IActionResult OnPostDeleteOrder()
        {
            try
            {
                if (PurchaseOrder == null)
                    throw new ArgumentNullException("No Purchase Order specified. Please try again.");

                _purchasingServices.DeleteActiveOrder(PurchaseOrder);

                FeedbackMessage = $"Order {PurchaseOrder.PurchaseOrderNumber} deleted.";
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                RepopulateFields();
                return Page();
            }

            return RedirectToPage(new { EmployeeID = EmployeeID, VendorID = VendorID });
        }

        public IActionResult OnPostClear()
        {
            VendorID = null;
            return RedirectToPage(new { EmployeeID = EmployeeID, VendorID = VendorID });
        }

        public void OnPostAddVendorItem()
        {
            if (vendorPartToMove <= 0)
            {
                ErrorMessage = "Unable to process your request. PartID is missing.";
            }
            else
            {
                var found = VendorInventory.Where(x => x.PartID == vendorPartToMove).SingleOrDefault();
                if (found != null)
                {
                    try
                    {
                        if (found.ReorderLevel - (found.QuantityOnHand + found.QuantityOnOrder) < 0)
                            found.QuantityToOrder = (found.ReorderLevel - (found.QuantityOnHand + found.QuantityOnOrder)) * -1;
                        else
                            found.QuantityToOrder = found.ReorderLevel - (found.QuantityOnOrder + found.QuantityOnHand);
                        VendorInventory.Remove(found);
                        CurrentOrder.Add(found);
                        CurrentOrder.Sort((x,y) => x.PartID.CompareTo(y.PartID));
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = GetInnerException(ex).Message;
                    }
                }
            }
            RepopulateFields();
        }

        public void OnPostRemoveOrderItem()
        {
            if(partToMove <= 0)
            {
                ErrorMessage = "Unable to process your request. PartID is missing.";
            }
            else
            {
                var found = CurrentOrder.Where(x => x.PartID == partToMove).SingleOrDefault();
                if (found != null)
                {
                    try
                    {
                        CurrentOrder.Remove(found);
                        VendorInventory.Add(found);
                        VendorInventory.Sort((x, y) => x.PartID.CompareTo(y.PartID));
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = GetInnerException(ex).Message;
                    }
                }
            }
            _ = GetActiveEmployee();
            Thread.Sleep(500);
            PopulateVendorList();
            GetVendorInfo();
            GetPurchaseOrder();
            GetOrderTotals();
        }

        public void OnPostRefreshOrderItem()
        {
            RepopulateFields();
        }

        public void GetPurchaseOrder()
        {
            PurchaseOrder = _purchasingServices.GetPurchaseOrder((int)VendorID);
        }

        public void GetOrderTotals()
        {
            const decimal GST = 0.05m;
            subtotal = 0;
            gst = 0;
            total = 0;
            foreach (var item in CurrentOrder)
            {
                if (item.QuantityToOrder != null)
                {
                    subtotal += item.PurchasePrice * (decimal)item.QuantityToOrder;
                    gst += (item.PurchasePrice * GST) * (decimal)item.QuantityToOrder;
                }

                total = subtotal + gst;
            }
        }
        
        #region Vendor Methods
        public void PopulateVendorList()
        {
            VendorList = _vendorServices.GetVendors();
        }

        public void GetVendorInfo()
        {
            Vendor = _vendorServices.GetVendorByID((int)VendorID);
        }
        #endregion

        public void RepopulateFields()
        {
            _ = GetActiveEmployee();
            Thread.Sleep(threadTimeout);
            PopulateVendorList();
            GetVendorInfo();
            GetPurchaseOrder();
            GetOrderTotals();
        }
        public async Task GetActiveEmployee()
        {
            AppUser = await _UserManager.FindByNameAsync(User.Identity.Name);
            EmployeeID = AppUser.EmployeeId.Value;
            EmployeeName = _Security.GetEmployeeName((int)EmployeeID);
        }

        private Exception GetInnerException(Exception ex)
        {
            while (ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}
