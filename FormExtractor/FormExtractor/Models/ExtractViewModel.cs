using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormExtractor.Models
{
    public class ExtractViewModel
    {
        public ExtractViewModel()
        {
            Vendors = new List<Vendor>();
        }

        public ExtractViewModel(ApplicationUser currentUser): this()
        {
            User = currentUser;
        }

        public List<Vendor> Vendors { get; set; }

        public ApplicationUser User { get; set; }
    }
}