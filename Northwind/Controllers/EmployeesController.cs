using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Northwind.DAL;
using Northwind.Models;
using System.Data.Entity.Infrastructure;
using AutoMapper;

namespace Northwind.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly NorthwindEntities _context;
        private UnitOfWork _unitOfWork;

        public EmployeesController()
        {
            _context = new NorthwindEntities();
            _unitOfWork = new UnitOfWork(_context);
        }

        // GET: Employees
        public ActionResult Index(string SelectedCountry)
        {
            EmployeeIndexVM employeeIndexVM = new EmployeeIndexVM();

            // We add a drop-down list to the Index page so that users can filter 
            // for a particular country; FirstOrDefault() is similar to select distinct.
            // See DrowpdownHelper.GetCountriesAll().

            //var employees = from e in _context.Employees
            //                orderby e.LastName
            //                select e;
            // var employees = unitOfWork.EmployeeRepository.Get(includeProperties: "Department");
            var employees = _unitOfWork.EmployeeRepository.Get(orderBy: q => q.OrderBy(e => e.LastName));

            // A SelectList collection containing all departments is passed to the view for the drop-down list.
            //ViewBag.SelectedDepartment = new SelectList(departments, "DepartmentID", "Name", SelectedDepartment);

            // The filter expression always returns true if nothing is selected in the drop-down list.
            if (!String.IsNullOrEmpty(SelectedCountry))
            {
                employees = employees
                    .Where(c => c.Country.Equals(SelectedCountry))
                    .OrderBy(e => e.LastName);
            }
            //employees = db.Employees
            //    .Where(c => String.IsNullOrEmpty(SelectedCountry) || c.Country.Equals(SelectedCountry))
            //    .OrderBy(e => e.LastName);

            //return View(await db.Employees.ToListAsync());
            //return View(await employees.ToListAsync());
            employeeIndexVM.EmployeeList = employees.ToList();

            return View(employeeIndexVM);
        }

        // GET: Employees/Details/5
        //public async Task<ActionResult> Details(int? id)
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Employee employee = await _context.Employees.FindAsync(id);
            Employee employee = _unitOfWork.EmployeeRepository.GetByID(id);
            var employeeVM = Mapper.Map<EmployeeVM>(employee);
            if (employeeVM == null)
            {
                return HttpNotFound();
            }
            return View(employeeVM);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            var viewModel = new EmployeeVM();
            return View(viewModel);
        }

        // POST: Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "LastName,FirstName,Title,BirthDate,HireDate,City,Region,PostalCode,Country,HomePhone,Extension,Notes,ReportsTo")] EmployeeVM employeeVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //throw new RetryLimitExceededException("Testing Error");
                    var newEmployee = Mapper.Map<Employee>(employeeVM);

                    //_context.Employees.Add(newEmployee);
                    //await _context.SaveChangesAsync();

                    _unitOfWork.EmployeeRepository.Insert(newEmployee);
                    _unitOfWork.Save();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException rex)
                {
                    // Log the error
                    // System.Reflection.MethodBase.GetCurrentMethod().ToString() will return entire method signature
                    //Logger.Log(rex, "Error creating new record in " +
                    //    this.GetType().FullName + "." +
                    //    System.Reflection.MethodBase.GetCurrentMethod().Name);
                    Logger.LogError(rex, "Error creating new record in " + 
                        this.GetType().FullName + "." +
                        System.Reflection.MethodBase.GetCurrentMethod().Name);
                    ModelState.AddModelError("", "Unable to create record. Try again, and if the problem persists see your system administrator.");
                }
            }

            return View(employeeVM);
        }

        // GET: Employees/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //Employee employee = await _context.Employees.FindAsync(id);
            Employee employee = _unitOfWork.EmployeeRepository.GetByID(id);
            var employeeVM = Mapper.Map<EmployeeVM>(employee);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employeeVM);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "EmployeeID,LastName,FirstName,Title,BirthDate,HireDate,City,Region,PostalCode,Country,HomePhone,Extension,Notes,ReportsTo")] EmployeeVM employeeVM)
        {
            if (employeeVM.EmployeeID <= 0)
                ModelState.AddModelError("EmployeeID", "EmployeeID is not valid.");

            if (ModelState.IsValid)
            {
                try
                {
                    var employee = Mapper.Map<Employee>(employeeVM);
                    //_context.Entry(employee).State = EntityState.Modified;
                    //await _context.SaveChangesAsync();

                    _unitOfWork.EmployeeRepository.Update(employee);
                    _unitOfWork.Save();

                    return RedirectToAction("Index");
                }
                catch (RetryLimitExceededException rex)
                {
                    // Log the error 
                    Logger.LogError(rex, "Error saving record in " +
                        this.GetType().FullName + "." +
                        System.Reflection.MethodBase.GetCurrentMethod().Name);
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(employeeVM);
        }

        // GET: Employees/Delete/5
        //public async Task<ActionResult> Delete(int? id, bool? saveChangesError = false)
        public ActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            //Employee employee = await _context.Employees.FindAsync(id);
            Employee employee = _unitOfWork.EmployeeRepository.GetByID(id);
            var employeeVM = Mapper.Map<EmployeeVM>(employee);
            if (employeeVM == null)
            {
                return HttpNotFound();
            }
            return View(employeeVM);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                //Employee employee = await _context.Employees.FindAsync(id);
                //_context.Employees.Remove(employee);
                //await _context.SaveChangesAsync();

                Employee employee = _unitOfWork.EmployeeRepository.GetByID(id);
                _unitOfWork.EmployeeRepository.Delete(id);
                _unitOfWork.Save();
            }
            catch (RetryLimitExceededException rex)
            {
                // Log the error
                Logger.LogError(rex, "Error deleting record in " +
                    this.GetType().FullName + "." +
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

