#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional namespaces
using ServicingSystem.DAL;
using ServicingSystem.ViewModels;
#endregion

namespace ServicingSystem.BLL
{
    public class CustomerServices
    {
        #region Constructor and Context Dependancy
        private readonly ServicingDbContext _context;

        internal CustomerServices(ServicingDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Query
        public List<CustomerListBy> GetCustomersByName(string searcharg,
                                                       int pagenumber,
                                                       int pagesize,
                                                       out int totalcount)
        {
            if(string.IsNullOrWhiteSpace(searcharg))
            {
                throw new ArgumentNullException("No search argument has been given.");
            }

            List<CustomerListBy> info = _context.Customers
                                                        .Where(c => c.FirstName.Contains(searcharg) || c.LastName.Contains(searcharg))
                                                        .Select(c => new CustomerListBy
                                                        {
                                                            CustomerID = c.CustomerID,
                                                            FullName = c.FirstName + " " + c.LastName,
                                                            Phone = c.ContactPhone,
                                                            Address = c.Address + " " + c.City
                                                        })
                                                        .OrderBy(c => c.FullName)
                                                        .ToList();
            totalcount = info.Count();
            int skipRows = (pagenumber - 1) * pagesize;
            return info.Skip(skipRows).Take(pagesize).ToList();
        }
        #endregion
    }
}
