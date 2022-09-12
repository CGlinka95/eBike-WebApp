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
    public class VehicleServices
    {
        #region Constructor and Context Dependancy
        private readonly ServicingDbContext _context;

        internal VehicleServices(ServicingDbContext context)
        {
            _context = context;
        }
        #endregion

        #region Query
        public List<VehicleList> GetVehiclesByID(int customerid)
        {
            List<VehicleList> info = _context.CustomerVehicles
                                                    .Where(cv => cv.CustomerID == customerid)
                                                    .Select(cv => new VehicleList
                                                    {
                                                        VIN = cv.VehicleIdentification,
                                                        MakeModel = cv.Make + ", " + cv.Model,
                                                    })
                                                    .OrderBy(cv => cv.MakeModel)
                                                    .ToList();
            return info.ToList();
        }
        #endregion
    }
}
