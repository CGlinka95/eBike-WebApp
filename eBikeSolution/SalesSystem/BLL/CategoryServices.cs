using SalesSystem.DAL;
using SalesSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalesSystem.BLL
{
    public class CategoryServices
    {
        #region Constructor and Context Dependency
        private readonly SalesDbContext _context;
        internal CategoryServices(SalesDbContext context)
        {
            _context = context;
        }
        #endregion

        // Get list of Categories for SelectionList
        public List<SelectionList> GetAllCategories()
        {
            List<SelectionList> info = _context.Categories
                                        .Select(x => new SelectionList
                                        {
                                            ValueID = x.CategoryID,
                                            DisplayText = x.Description
                                        })
                                        .ToList();
            return info;
        }
    }
}
