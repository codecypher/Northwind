using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Northwind.Models
{
    public class CustomerIndexVM
    {
        public string SearchString { get; set; }
        public PagedList.IPagedList<Northwind.Models.Customer> CustomerPagedList { get; set; }
        public string CompanyNameSortParm { get; set; }
        public string ContactNameSortParm { get; set; }
        public string SortOrder { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set;}
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}