#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicingSystem.ViewModels
{
    public class StandardJobList
    {
        public int StandardJobID { get; set; }
        public string StandardDescription { get; set; }
        public decimal StandardHours { get; set; }
    }
}
