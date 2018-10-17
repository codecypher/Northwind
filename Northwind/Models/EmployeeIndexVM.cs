using Northwind.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Northwind.Models
{
    public class EmployeeIndexVM
    {
        public List<Employee> EmployeeList { get; set; }

        [Display(Name = "Country")]
        public string SelectedCountry { get; set; }

        public IEnumerable<SelectListItem> Countries
        {
            get { return DropdownHelper.GetCountriesAll(); }
        }
    }
}