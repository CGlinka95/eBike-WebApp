using AppSecurity.BLL;
using eBikeWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecievingSystem.BLL;
using RecievingSystem.ViewModels;

namespace eBikeWebApp.Pages.RecievingPages
{
    public class RecievingModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SecurityService _Security;
        private readonly PurchaseOrderServices _purchaseOrderServices;
        public ApplicationUser AppUser { get; set; }
        public string FeedBackMessage { get; set; }
        [BindProperty]
        public string EmployeeName { get; set; }
        [BindProperty]
        public List<OutStandingOrder>? outStandingOrders { get; set; }
        public RecievingModel(UserManager<ApplicationUser> userManager, SecurityService security, PurchaseOrderServices services)
        {
            _UserManager = userManager;
            _Security = security;
            _purchaseOrderServices = services;
        }
        public async Task OnGet(string feedBackMessage)
        {
            FeedBackMessage = feedBackMessage;
            try
            {
                outStandingOrders = _purchaseOrderServices.fetchOutStandingOrders();

                foreach (var order in outStandingOrders)
                {
                    Console.WriteLine("ORDER: " + order.VendorName);

                }
                AppUser = await _UserManager.FindByNameAsync(User.Identity.Name);
                EmployeeName = _Security.GetEmployeeName(AppUser.EmployeeId.Value);
            }
            catch
            {
                EmployeeName = "Nobody, because nobody logged in";
            }
        }
        public IActionResult OnPostViewOrder(string PurchaseOrderID)
        {
            if (PurchaseOrderID == "")
            {
                throw new Exception("VieW Order Button returned a value that is empty!");
            }
            if (!int.TryParse(PurchaseOrderID, out int orderID))
            {
                throw new ArgumentException("View Order Button was pressed, and value was not a integer, OnPostViewOrder");
            }

            _purchaseOrderServices.clearUnOrderedItemsTable();
            Console.WriteLine("View Order Button Value: " + PurchaseOrderID);
            return RedirectToPage("/RecievingPages/RecievingOrderDetail", new { OrderID = PurchaseOrderID });
        }
    }
}
