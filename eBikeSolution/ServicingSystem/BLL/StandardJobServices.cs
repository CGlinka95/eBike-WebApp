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
    public class StandardJobServices
    {
        #region Constructor and Context Dependancy
        private readonly ServicingDbContext _context;

        internal StandardJobServices(ServicingDbContext context)
        {
            _context = context;
        }
        #endregion

        public List<StandardJobList> GetStandardJobList ()
        {
            List<StandardJobList> info = _context.StandardJobs
                                                 .Select(sj => new StandardJobList
                                                 {
                                                     StandardJobID = sj.StandardJobID,
                                                     StandardDescription = sj.Description,
                                                     StandardHours = sj.StandardHours
                                                 })
                                                 .OrderBy(sj => sj.StandardDescription)
                                                 .ToList();
            return info;
        }
    }
}
