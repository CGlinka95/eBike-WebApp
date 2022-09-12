using AppSecurity.BLL;
using eBikeWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecievingSystem.BLL;
using RecievingSystem.ViewModels;

namespace eBikeWebApp.Pages.RecievingPages
{
    public class RecievingOrderDetailModel : PageModel
    {

        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SecurityService _Security;
        private readonly RecievingOrderDetailsServices _recieveOrderDetailServices;
        public ApplicationUser AppUser { get; set; }
        public string EmployeeName { get; set; }
        #region User feedback
        public string FeedBackMessage { get; set; }

        public string ErrorMessage { get; set; }
        public List<String> ErrorDetails { get; set; } = new();
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        #endregion
        #region Form values
        [BindProperty]
        public string ForceCloseReason { get; set; }
        [BindProperty(SupportsGet = true)]
        public List<RecievedItems> recievedItems { get; set; }

        #endregion
        #region Query variables
        [BindProperty(SupportsGet = true)]
        public OutStandingOrder? order { get; set; }
        [BindProperty]
        public List<OutStandingOrderDetail> orderDetails { get; set; }

        //[BindProperty]
        [BindProperty(SupportsGet = true)]
        public int? OrderId { get; set; }

        [BindProperty]
        public int? EmployeeId { get; set; }
        #endregion
        #region UnOrderedItem Properties
        [BindProperty]
        public string UnorderedDescription { get; set; }
        [BindProperty]
        public string UnorderedVendorNumber { get; set; }
        [BindProperty]
        public int UnorderedQuantity { get; set; }
        [BindProperty]
        public UnOrderedItemToAdd UnOrderedItemToAdd { get; set; } = new();
        [BindProperty(SupportsGet = true)]
        public List<UnOrderedItem> UnOrderedItems { get; set; }
        [BindProperty]
        public string UnOrderItemID { get; set; }
        #endregion
        public RecievingOrderDetailModel(UserManager<ApplicationUser> userManager, SecurityService security, RecievingOrderDetailsServices recieveOrderServices)
        {
            _UserManager = userManager;
            _Security = security;

            _recieveOrderDetailServices = recieveOrderServices;
            Console.WriteLine("Your OrderID in the RecievingOrderDetail Model Constructor: " + OrderId);

        }
        public async Task OnGetAsync(List<String> ErrorDetail, List<int?> recQty, List<int?> returnQty, List<string?> reasons)
        {
            ErrorDetails = ErrorDetail;
            this.recQty = recQty;
            this.returnQty = returnQty;
            this.reasons = reasons;
            UnOrderedItems = _recieveOrderDetailServices.FetchUnorderedItems();
            try
            {
                AppUser = await _UserManager.FindByNameAsync(User.Identity.Name);
                EmployeeName = _Security.GetEmployeeName(AppUser.EmployeeId.Value);
                EmployeeId = AppUser.EmployeeId.Value;
                order = new OutStandingOrder();

                order = _recieveOrderDetailServices.FetchOutStandingOrder(OrderId ?? -1);
                OrderId = order.PurchaseOrderID;
                orderDetails = _recieveOrderDetailServices.FetchOutStandingOrderDetailsByID(OrderId ?? -1);
                foreach (var order in orderDetails)
                {
                    Console.WriteLine($"Order ID:{order.PurchaseOrderDetailID}, Desc: {order.PartDescription}, OrignalQTY: {order.OustandingQtyOriginal}, OUstanding: {order.OustandingQtyOriginal}");
                }
                Console.WriteLine($"RecieveOrderDetails OnGetPurchase ORderID: {order.PurchaseOrderID}, EmployeeName: {EmployeeName}, Employee ID: {EmployeeId} ");
                Console.WriteLine("Order Vendor Name: " + order.VendorName ?? "order is null");
            }
            catch (Exception ex)
            {
                ErrorMessage = "SYSTEM ERROR: Something went wrong on our end! checkReceivingOrderDetail OnGet()";
            }
        }
        #region Recieve and Close Order Methods
        public IActionResult OnPostRecieve()
        {
            Console.WriteLine("OnPostRecieve: ");
            try
            {
                Console.WriteLine($"OnPostRecieve: OrderID: {OrderId ?? -1}, EmployeeID: {EmployeeId ?? -1}");
                Console.WriteLine("Recieved ITtems Bellow: ");
                foreach (var item in recievedItems)
                {
                    Console.WriteLine($" Part Description: {item.PartDescription} recQty: {item.QuantityRecieved} returned: {item.QuantityRecieved} reseason: {item.ReturnReason} Quantity OutStanding: {item.QuantityOutStanding} PartID: {item.PartID}");
                }
                _recieveOrderDetailServices.RecieveOrder(OrderId ?? -1, EmployeeId ?? -1, recievedItems);
                UnOrderedItems = _recieveOrderDetailServices.FetchUnorderedItems();
                if (UnOrderedItems.Count() != 0)
                {
                    try
                    {

                        _recieveOrderDetailServices.saveCartItems(UnOrderedItems, OrderId ?? -1, EmployeeId ?? -1);
                        _recieveOrderDetailServices.clearUnOrderedItemsTable();
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error in saving cart items!");
                    }

                }
                return RedirectToPage("/RecievingPages/Recieving", new { feedBackMessage = $"Purchase Order # {OrderId} Recieved!" });
            }
            catch (AggregateException ex)
            {

                Exception inner = ex;
                Console.WriteLine("ORDER ID PASSED IN: " + OrderId);
                ErrorMessage = "Unable to recieve Order!";
                foreach (var problem in ex.InnerExceptions)
                {
                    ErrorDetails.Add(problem.Message);
                    Console.WriteLine("error Message TEst: " + problem.Message);
                }
                OrderId = OrderId ?? -1;
                return RedirectToPage(new { OrderID = OrderId, OrderDetails = orderDetails, ErrorDetail = ErrorDetails });
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                ErrorMessage = $"Something wwent wrong: {ex.Message}";
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }
                ErrorMessage = inner.Message;
                return RedirectToPage(new { OrderID = OrderId });
            }
        }
        public IActionResult OnPostForceClose()
        {
            Console.WriteLine("OnPostForceClose Clicked!");
            try
            {
                Console.WriteLine($"OnPostRecieve: OrderID: {OrderId ?? -1}, EmployeeID: {EmployeeId ?? -1}");
                Console.WriteLine("Recieved ITtems Bellow: ");
                foreach (var item in recievedItems)
                {
                    Console.WriteLine($" Part Description: {item.PartDescription} recQty: {item.QuantityRecieved} returned: {item.QuantityRecieved} reseason: {item.ReturnReason} Quantity OutStanding: {item.QuantityOutStanding} PartID: {item.PartID}");
                }
                Console.WriteLine($"Force close reason: {ForceCloseReason}");
                _recieveOrderDetailServices.ForceRecieveOrder(OrderId ?? -1, EmployeeId ?? -1, recievedItems, ForceCloseReason);
                return RedirectToPage("/RecievingPages/Recieving", new { feedBackMessage = $"Purchase Order # {OrderId} Recieved!" });
            }
            catch (AggregateException ex)
            {

                Exception inner = ex;
                Console.WriteLine("ORDER ID PASSED IN: " + OrderId);
                ErrorMessage = "Unable to recieve Order!";
                foreach (var problem in ex.InnerExceptions)
                {
                    ErrorDetails.Add(problem.Message);
                    Console.WriteLine("error Message TEst: " + problem.Message);
                }
                OrderId = OrderId ?? -1;
                return RedirectToPage(new { OrderID = OrderId, OrderDetails = orderDetails, ErrorDetail = ErrorDetails });
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                ErrorMessage = "Something went Wrong!" + ex.Message;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }
                ErrorMessage = inner.Message;
                return RedirectToPage(new { OrderID = OrderId });
            }
        }
        #endregion
        #region UnOrderedITems methods
        public List<int?> recQty { get; set; } = new List<int?>();
        public List<int?> returnQty { get; set; } = new List<int?>();
        public List<string?> reasons { get; set; } = new List<string?>();
        public void saveUserInputs()
        {
            UnOrderedItemToAdd = new();

            foreach (var item in recievedItems)
            {
                recQty.Add(item.QuantityRecieved);
                returnQty.Add(item.QuantityReturned);
                reasons.Add(item.ReturnReason);
            }
        }
        public IActionResult OnPostInsertUnorderedItem()
        {

            saveUserInputs();

            try
            {
                UnOrderedItemToAdd.Description = UnorderedDescription;
                UnOrderedItemToAdd.Quantity = UnorderedQuantity;
                UnOrderedItemToAdd.VendorPartNumber = UnorderedVendorNumber;
                _recieveOrderDetailServices.InsertUnOrderedItem(UnOrderedItemToAdd);
                return RedirectToPage(new { OrderID = OrderId, recQty = recQty, returnQty = returnQty, reasons = reasons, ErrorDetail = ErrorDetails });
            }
            catch (AggregateException ex)
            {

                Exception inner = ex;

                ErrorMessage = "Unable to store UnOrderedItem";
                foreach (var problem in ex.InnerExceptions)
                {
                    ErrorDetails.Add(problem.Message);
                    Console.WriteLine("error Message TEst: " + problem.Message);
                }
                return RedirectToPage(new { OrderID = OrderId, recQty = recQty, returnQty = returnQty, reasons = reasons, ErrorDetail = ErrorDetails });
            }
            catch (Exception ex)
            {
                Exception inner = ex;
                ErrorMessage = "Something went Wrong!" + ex.Message;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                }
                ErrorMessage = inner.Message;
                return RedirectToPage(new { OrderID = OrderId, recQty = recQty, returnQty = returnQty, reasons = reasons, ErrorDetail = ErrorDetails });
            }
        }
        public IActionResult OnPostDeleteUnorderedItem(string CartID)
        {
            saveUserInputs();
            int id = -1;
            Console.WriteLine("CARD ID: " + CartID);
            if (!int.TryParse(CartID, out id))
            {
                return BadRequest();
            }
            else
            {
                _recieveOrderDetailServices.deleteUnOrderedItem(id);
                return RedirectToPage(new { OrderID = OrderId, recQty = recQty, returnQty = returnQty, reasons = reasons, ErrorDetail = ErrorDetails });
            }
        }
        public IActionResult OnPostClearUnOrderedItem()
        {

            saveUserInputs();
            return RedirectToPage(new { OrderID = OrderId, recQty = recQty, returnQty = returnQty, reasons = reasons });

        }
        #endregion

    }
}
