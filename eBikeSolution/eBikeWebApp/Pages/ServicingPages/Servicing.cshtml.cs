using eBikeWebApp.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional namespaces
using AppSecurity.BLL;
using ServicingSystem.ViewModels;
using WebApp.Helpers;
using ServicingSystem.BLL;
#endregion

namespace eBikeWebApp.Pages.ServicingPages
{
    public class ServicingModel : PageModel
    {
        #region Private variables and DI Cunstructor
        private readonly UserManager<ApplicationUser> _UserManager;
        private readonly SecurityService _Security;
        private readonly CustomerServices _customerServices;
        private readonly VehicleServices _vehicleServices;
        private readonly JobServices _jobServices;
        private readonly StandardJobServices _standardJobServices;

        public ServicingModel(UserManager<ApplicationUser> userManager,
                                   SecurityService security,
                                   CustomerServices customerservices,
                                   VehicleServices vehicleservices,
                                   JobServices jobservices,
                                   StandardJobServices standardjobservices)
        {
            _UserManager = userManager;
            _Security = security;
            _customerServices = customerservices;
            _vehicleServices = vehicleservices;
            _jobServices = jobservices;
            _standardJobServices = standardjobservices;
        }
        #endregion

        #region FeedBack and ErrorHandling
        [TempData]
        public string FeedBackMessage { get; set; }
        public string ErrorMsg { get; set; }

        public bool HasFeedBack => !string.IsNullOrWhiteSpace(FeedBackMessage);
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMsg);

        public List<string> ErrorDetails { get; set; } = new();
        public List<Exception> Errors { get; set; } = new();
        #endregion

        #region Security
        public ApplicationUser AppUser { get; set; }
        public string EmployeeName { get; set; }

        //optionally you can use this variable in routing
        [BindProperty(SupportsGet = true)]
        public int? employeeID { get; set; }
        #endregion

        #region Paginator variables
        private const int PAGE_SIZE = 10;
        public Paginator Pager { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? currentpage { get; set; }
        #endregion

        #region Bound Properties & Lists
        [BindProperty(SupportsGet = true)]
        public string? searchArg { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? customerID { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? vehicleIdentification { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? standardJobID { get; set; }


        [BindProperty]
        public string serviceDescription { get; set; }
        [BindProperty]
        public decimal serviceHours { get; set; }
        [BindProperty]
        public string customerComments { get; set; }
        [BindProperty]
        public string couponIDValue { get; set; }
        [BindProperty]
        public string CustomerName { get; set; }


        [BindProperty]
        public List<CustomerListBy> CustomerByName { get; set; }

        [BindProperty]
        public List<VehicleList> CustomerVehicleList { get; set; } = new();

        [BindProperty]
        public List<StandardJobList> StandardJobList { get; set; } = new();

        [BindProperty]
        public List<ServiceInfo> ListServicesInfo { get; set; } = new();
        #endregion

        public async Task OnGet()
        {
            try
            {
                AppUser = await _UserManager.FindByNameAsync(User.Identity.Name);
                employeeID = AppUser.EmployeeId.Value;
                EmployeeName = _Security.GetEmployeeName(AppUser.EmployeeId.Value);
            }
            catch
            {
                EmployeeName = "Nobody, because nobody logged in";
            }

            if(employeeID.HasValue && employeeID > 0)
            {
                GetCustomerInfo();
            }
            else
            {
                Errors.Add(new Exception("Employee must log in before creating a new job."));
            }

            if(customerID.HasValue && customerID > 0)
            {
                GetSelectedCustomerVehicle();
                GetStandardJobList();
            }
        }

        public async Task GetActiveEmployee()
        {
            AppUser = await _UserManager.FindByNameAsync(User.Identity.Name);
            employeeID = AppUser.EmployeeId.Value;
            EmployeeName = _Security.GetEmployeeName(AppUser.EmployeeId.Value);
        }

        public void GetCustomerInfo()
        {
            if (!string.IsNullOrWhiteSpace(searchArg))
            {
                int totalrows = 0;
                int pagenumber = currentpage.HasValue ? currentpage.Value : 1;
                PageState current = new(pagenumber, PAGE_SIZE);
                CustomerByName = _customerServices.GetCustomersByName(searchArg.Trim(),
                                pagenumber, PAGE_SIZE, out totalrows);
                Pager = new(totalrows, current);
            }
        }

        public void GetSelectedCustomerVehicle()
        {
            CustomerName = CustomerByName.Find(x => x.CustomerID == customerID).FullName; 
            CustomerVehicleList = _vehicleServices.GetVehiclesByID((int)customerID);
            CustomerVehicleList.Sort((x, y) => x.MakeModel.CompareTo(y.MakeModel));
        }

        public void GetStandardJobList()
        {
            StandardJobList = _standardJobServices.GetStandardJobList();
            StandardJobList.Sort((x, y) => x.StandardDescription.CompareTo(y.StandardDescription));
        }

        public IActionResult OnPostCustomerSearch()
        {
            _ = GetActiveEmployee();
            Thread.Sleep(1000);
            employeeID = employeeID;
            try
            {
                if(string.IsNullOrWhiteSpace(searchArg))
                {
                    Errors.Add(new Exception("Customer search argument not given."));
                }
                if(Errors.Any())
                {
                    throw new AggregateException(Errors);
                }

                return RedirectToPage(new
                {
                    employeeID = employeeID,
                    searchArg = searchArg.Trim()
                });
            }
            catch(AggregateException ex)
            {
                ErrorMsg = "Unable to process search request";
                foreach(var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                return Page();
            }
            catch(Exception ex)
            {
                ErrorMsg = GetInnerException(ex).Message;
                return Page();
            }
        }

        public IActionResult OnPostSelectedCustomerVehicle()
        {
            return RedirectToPage(new
            {
                employeeID = employeeID,
                searchArg = searchArg.Trim(),
                customerID = customerID,
                vehicleIdentification = vehicleIdentification
            });
        }

        public IActionResult OnPostListServiceInfo()
        {
            try
            {
                if(string.IsNullOrWhiteSpace(serviceDescription))
                {
                    throw new Exception("Service description must be supplied before adding the service.");
                }

                if (serviceHours <= 0)
                {
                    throw new Exception("Service hours must be supplied before adding the service.");
                }

                ListServicesInfo.Add(new ServiceInfo(serviceDescription, serviceHours, customerComments, couponIDValue));

                _ = GetActiveEmployee();
                Thread.Sleep(1000);
                employeeID = employeeID;
                GetCustomerInfo();
                GetSelectedCustomerVehicle();
                GetStandardJobList();
                return Page();
            }
            catch (AggregateException ex)
            {
                ErrorMsg = "Unable to process search request";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMsg = GetInnerException(ex).Message;
                return Page();
            }
        }

        public IActionResult OnPostSave()
        {
            try
            {
                if(employeeID == 0)
                {
                    throw new Exception("Must have an Employee ID before creating a new job.");
                }

                if(string.IsNullOrWhiteSpace(vehicleIdentification))
                {
                    throw new Exception("Must have a Vehicle ID before creating a new job.");
                }

                if(ListServicesInfo.Count == 0)
                {
                    throw new Exception("Must have at least one service in the services list before creating a new job.");
                }

                _jobServices.Job_RecordJob((int)employeeID, vehicleIdentification, ListServicesInfo);
                FeedBackMessage = "Job has been saved";

                _ = GetActiveEmployee();
                Thread.Sleep(1000);
                employeeID = employeeID;
                GetStandardJobList();

                return RedirectToPage(new
                {
                    employeeID = employeeID,
                    searchArg = searchArg,
                    customerID = customerID,
                    vehicleIdentification = vehicleIdentification
                });

            }
            catch(AggregateException ex)
            {
                ErrorMsg = "Unable to process your request:";
                foreach (var problem in ex.InnerExceptions)
                    ErrorDetails.Add(problem.Message);
                return Page();
            }
            catch(Exception ex)
            {
                ErrorMsg = GetInnerException(ex).Message;
                return Page();
            }
        }

        private Exception GetInnerException(Exception ex)
        {
            while(ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}
