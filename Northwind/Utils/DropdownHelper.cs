using Northwind.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Northwind.DAL;

namespace Northwind.Utils
{
    // Helper functions for creating drop-down lists.
    // DropDownListFor with ASP.NET MVC
    // http://odetocode.com/blogs/scott/archive/2013/03/11/dropdownlistfor-with-asp-net-mvc.aspx
    // http://odetocode.com/blogs/scott/archive/2010/01/18/drop-down-lists-and-asp-net-mvc.aspx
    public static class DropdownHelper
    {
        private static NorthwindEntities _context = new NorthwindEntities();

        // Return drop-down list of countries
        public static IEnumerable<SelectListItem> GetCountries()
        {
            // Create LINQ query.
            // FirstOrDefault() is similar to select distinct.
            var countries = from employee in _context.Employees
                            group employee by employee.Country into g
                            orderby g.Key
                            select g.FirstOrDefault();

            //return departments.Select(x => new SelectListItem { Text = x.departmentName, Value = x.departmentId.ToString() }));

            // Project each element of query into IQueryable<SelectListItem>.
            // IQueryable interface inherits from IEnumerable.
            // http://stackoverflow.com/questions/252785/what-is-the-difference-between-iqueryablet-and-ienumerablet#252857
            return countries.Select(x => new SelectListItem { Text = x.Country, Value = x.Country });
        }

        // Return drop-down list of countries plus "All" item
        public static IEnumerable<SelectListItem> GetCountriesAll()
        {
            // Create LINQ query.
            // FirstOrDefault() is similar to select distinct.
            var countries = from employee in _context.Employees
                            group employee by employee.Country into g
                            orderby g.Key
                            select g.FirstOrDefault();

            // Project each element of query into List<SelectListItem>.
            var countriesList = countries.Select(x => new SelectListItem {
                Text = x.Country,
                Value = x.Country
            }).ToList();
            
            // Insert "All" at beginning of List.
            countriesList.Insert(0, new SelectListItem { Text = "All", Value = "" });

            return countriesList;
        }

        // Return drop-down list of countries plus "null" item
        public static IEnumerable<SelectListItem> GetCountriesNull()
        {
            // Create LINQ query.
            // FirstOrDefault() is similar to select distinct.
            var countries = from employee in _context.Employees
                            group employee by employee.Country into g
                            orderby g.Key
                            select g.FirstOrDefault();
            //return departments.Select(x => new SelectListItem { Text = x.departmentName, Value = x.departmentId.ToString() }));

            // Project each element of query into List<SelectListItem>.
            var countriesList = countries.Select(x => new SelectListItem {
                Text = x.Country,
                Value = x.Country
            }).ToList();

            // Insert null value at beginning of List.
            countriesList.Insert(0, new SelectListItem { Text = "", Value = null });
            return countriesList;
        }

        // Returnd drop-down list of employees 
        public static IEnumerable<SelectListItem> GetEmployees()
        {
            //var departmentsQuery = _unitOfWork.DepartmentRepository.Get(
            //    orderBy: q => q.OrderBy(d => d.Name));

            // Create LINQ query.
            var employees = from e in _context.Employees
                            orderby e.LastName
                            select e;

            // Project each element of query into List<SelectListItem>.
            return employees.Select(x => new SelectListItem {
                Text = x.FirstName + " " + x.LastName,
                Value = x.EmployeeID.ToString()
            });
        }

        // Return drop-down list of employees plus "Null" item
        public static IEnumerable<SelectListItem> GetEmployeesNull()
        {
            // Create LINQ query.
            var employees = from e in _context.Employees
                            orderby e.LastName
                            select e;

            // Project each element of query into List<SelectListItem>.
            var employeesList = employees.Select(x => new SelectListItem {
                Text = x.FirstName + " " + x.LastName,
                Value = x.EmployeeID.ToString()
            }).ToList();

            // Insert null value at beginning of List.
            employeesList.Insert(0, new SelectListItem { Text = "", Value = null });

            return employeesList;
        }
    }
}