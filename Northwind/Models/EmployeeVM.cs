using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Northwind.Utils;
using System.Web.Mvc;

namespace Northwind.Models
{
    public class EmployeeVM
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeID { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "Last Name cannot be longer than 20 characters.")]
        //[StringLength(50, MinimumLength = 3)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(10, ErrorMessage = "First Name cannot be longer than 10 characters.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [StringLength(30, ErrorMessage = "Title cannot be longer than 30 characters.")]
        public string Title { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Birth Date")]
        public Nullable<System.DateTime> BirthDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public Nullable<System.DateTime> HireDate { get; set; }

        [StringLength(15, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string City { get; set; }

        [StringLength(15, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string Region { get; set; }

        [StringLength(10, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [StringLength(15, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string Country { get; set; }

        [StringLength(24, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        [Display(Name = "Home Phone")]
        public string HomePhone { get; set; }

        [StringLength(4, ErrorMessage = "{0} cannot be longer than {1} characters.")]
        public string Extension { get; set; }

        [DataType(DataType.MultilineText)]
        public string Notes { get; set; }

        [Display(Name = "Reports To")]
        public int? ReportsTo { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }

        public virtual ICollection<Employee> EmployeesReportingTo { get; set; }

        public virtual Employee EmployeeReportsTo { get; set; }

        public IEnumerable<SelectListItem> EmployeeList
        {
            get { return DropdownHelper.GetEmployeesNull(); }
        }
    }
}