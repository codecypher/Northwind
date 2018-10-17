using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Northwind.Models
{
    public class CustomerVM
    {
        [Required(ErrorMessage = "{0} is required.")]
        [RegularExpression(@"^[a-z,A-Z]{5}$", ErrorMessage = "CustomerID must be 5 characters.")]
        public string CustomerID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(40, ErrorMessage = "{0} cannot be more than {1} characters.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [StringLength(30, ErrorMessage = "{0} cannot be more than {1} characters.")]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [StringLength(30, ErrorMessage = "{0} cannot be more than {1} characters.")]
        [Display(Name = "Contact Title")]
        public string ContactTitle { get; set; }

        [StringLength(60, ErrorMessage = "{0} cannot be more than {1} characters.")]
        public string Address { get; set; }

        [StringLength(15, ErrorMessage = "{0} cannot be more than {1} characters.")]
        public string City { get; set; }

        [StringLength(15, ErrorMessage = "{0} cannot be more than {1} characters.")]
        public string Region { get; set; }

        [StringLength(10, ErrorMessage = "{0} cannot be more than {1} characters.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [StringLength(15, ErrorMessage = "{0} cannot be more than {1} characters.")]
        public string Country { get; set; }

        [StringLength(24, ErrorMessage = "{0} cannot be more than {1} characters.")]
        public string Phone { get; set; }

        [StringLength(24, ErrorMessage = "{0} cannot be more than {1} characters.")]
        public string Fax { get; set; }

        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public int? SelectedOrderID { get; set; }

        public List<Order> Orders { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}