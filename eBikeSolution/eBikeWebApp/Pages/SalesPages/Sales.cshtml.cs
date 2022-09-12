using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Added using
using eBikeWebApp.Data;
using Microsoft.AspNetCore.Identity;
using AppSecurity.BLL;
using SalesSystem.ViewModels;
using SalesSystem.BLL;
#endregion

namespace eBikeWebApp.Pages.SalesPages
{
    public class SalesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SecurityService _Security;
        private readonly CategoryServices _categoryServices;
        private readonly PartServices _partServices;

        public ApplicationUser AppUser { get; set; }
        public string EmployeeName { get; set; }
        public SalesModel(UserManager<ApplicationUser> userManager, SecurityService security, CategoryServices categoryServices, PartServices partServices)
        {
            _UserManager = userManager;
            _Security = security;
            _categoryServices = categoryServices;
            _partServices = partServices;
        }

        public List<SelectionList> CategoryList { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? CategoryID { get; set; }
        public List<SelectionList> PartList { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? PartID { get; set; }
        //public PartForSale Part { get; set; }===============================================================================
        [BindProperty]
        public List<PartsListBy> Part {get; set;}
        [BindProperty]
        public List<PartsListBy> CartParts { get; set; } = new();
        public int SaleID { get; set; }
        public string PaymentType { get; set; }
        public DateTime SaleDate { get; set; }
        public int CouponID { get; set; }
        public int EmployeeID { get; set; }

        public async Task OnGet()
        {
            _ = EmployeeInfo();
            PopulateCategoryList();

            if (CategoryID.HasValue && CategoryID.Value > 0)
            {
                PopulatePartList();
            }

            if (PartID.HasValue && PartID > 0)
            {
                Part = _partServices.PartInfo((int)PartID);
            }
        }

        #region Security
        public async Task EmployeeInfo()
        {
            try
            {
                AppUser = await _UserManager.FindByNameAsync(User.Identity.Name);
                EmployeeName = _Security.GetEmployeeName(AppUser.EmployeeId.Value);
            }
            catch
            {
                EmployeeName = "Nobody, because nobody logged in";
            }
        }
        #endregion

        #region Posts
        // Populate the parts list using the given category
        public IActionResult OnPostGetParts()
        {
            return RedirectToPage(new
            {
                CategoryID = CategoryID
            });
        }

        // Add the part to the cart
        public IActionResult OnPostAddPart()
        {
            //if (Part != null)
            //{
            //    CartParts.Add(Part);
            //}
            //RepopulateFields();
            return RedirectToPage(new
            {
                CategoryID = CategoryID,
                PartID = PartID,
                CartParts = CartParts
            });
        }

        // Save the cart as a new sale
        public IActionResult OnPostNewSale()
        {
            try
            {
                _partServices.CreateSale(SaleID, EmployeeID, PaymentType, SaleDate, CouponID, CartParts);

                return RedirectToPage(new
                {
                    SaleID = SaleID,
                    EmployeeID = EmployeeID
                });
            }
            catch(Exception ex)
            {
                return Page();
            }
        }
        #endregion

        #region Methods
        public void PopulateCategoryList()
        {
            CategoryList = _categoryServices.GetAllCategories();
        }

        public void PopulatePartList()
        {
            if (CategoryID.HasValue && CategoryID > 0)
            {
                PartList = _partServices.GetPartsInCategory((int)CategoryID);
            }
        }

        public void RepopulateFields()
        {
            _ = EmployeeInfo();
            PopulateCategoryList();
            PopulatePartList();
        }
        #endregion
    }
}
