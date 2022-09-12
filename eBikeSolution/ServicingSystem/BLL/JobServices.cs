using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional namespaces
using ServicingSystem.DAL;
using ServicingSystem.Entities;
using ServicingSystem.ViewModels;
#endregion

namespace ServicingSystem.BLL
{
    public class JobServices
    {
        #region Constructor and Context Dependancy
        private readonly ServicingDbContext _context;

        internal JobServices(ServicingDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Command
        public void Job_RecordJob(int employeeid, string vehicleid, List<ServiceInfo> servicelistitem)
        {
            #region Global Variable
            const decimal shopRate = 50.00m;
            #endregion

            #region Error List
            List<Exception> errorlist = new List<Exception>();
            #endregion

            #region Parameter Validation
            if (employeeid == 0)
            {
                throw new ArgumentNullException("Employee ID is missing. Employee must log in before creating a new job.");
            }

            if(vehicleid == null)
            {
                throw new ArgumentNullException("Vehicle Identification Number is missing. Please select a vehicle.");
            }

            if(servicelistitem.Count == 0)
            {
                throw new ArgumentNullException("List of services for the job cannot be less than or equal to 0. Please add at least 1 service to the service list.");
            }
            #endregion

            #region Parameter Exists
            // Check if employeeid exists
            Employee? employeeexists = _context.Employees
                                              .Where(e => e.EmployeeID == employeeid)
                                              .FirstOrDefault();
            if(employeeexists == null)
            {
                throw new ArgumentNullException("That employee does not exists, please try logging in again with valid credentials.");
            }

            // Check if vehicleid exists
            CustomerVehicle? vehicleexists = _context.CustomerVehicles
                                            .Where(v => v.VehicleIdentification == vehicleid)
                                            .FirstOrDefault();
            if(vehicleexists == null)
            {
                throw new ArgumentNullException("That vehicle does not exist, please try selecting a different vehicle.");
            }
            #endregion

            #region Update ServiceItem
            Job newJob = new Job();
            newJob.JobDateIn = DateTime.Now;
            newJob.EmployeeID = employeeid;
            newJob.ShopRate = shopRate;
            newJob.VehicleIdentification = vehicleid;

            vehicleexists.Jobs.Add(newJob);
            #endregion

            #region Validation and Update
            Coupon? checkCoupon = new Coupon();

            foreach (var item in servicelistitem)
            {
                // Check if ServiceDescription is supplied
                if(string.IsNullOrWhiteSpace(item.ServiceDescription))
                {
                    throw new ArgumentNullException("Service description is a required field.");
                }

                //Check if ServiceHours are supplied
                if(item.ServiceHours <= 0)
                {
                    throw new ArgumentNullException("Service hours is a required field and must be greater than 0.");
                }

                if(!string.IsNullOrWhiteSpace(item.CouponIDValue))
                {
                    checkCoupon = _context.Coupons
                                                 .Where(c => c.CouponIDValue == item.CouponIDValue)
                                                 .FirstOrDefault();
                    if(checkCoupon == null)
                    {
                        errorlist.Add(new Exception("Coupon does not exist, please try again")); 
                    }
                }

                JobDetail newJobDetail = new JobDetail();
                newJobDetail.JobID = newJob.JobID;
                newJobDetail.Description = item.ServiceDescription;
                newJobDetail.JobHours = item.ServiceHours;
                newJobDetail.Comments = item.CustomerComments;
                if(checkCoupon.CouponID > 0)
                {
                    if(DateTime.Compare(checkCoupon.EndDate, DateTime.Now) < 0)
                    {
                        errorlist.Add(new Exception("The expiry date on the coupon has passed. Please enter a valid coupon value."));
                    }
                    else
                    {
                        newJobDetail.CouponID = checkCoupon.CouponID; 
                    }
                }
                newJobDetail.StatusCode = "I";
                newJobDetail.EmployeeID = employeeid;
                newJob.JobDetails.Add(newJobDetail);
            }
            if(errorlist.Count > 0)
            {
                throw new AggregateException("Unable to process transaction", errorlist);
            }
            else
            {
                _context.SaveChanges();
            }
            #endregion
        }
        #endregion
    }
}
