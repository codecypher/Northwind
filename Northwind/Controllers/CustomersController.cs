using System;
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
using PagedList.EntityFramework;
using System.Data.Entity.Infrastructure;
using AutoMapper;

namespace Northwind.Controllers
{
    public class CustomersController : Controller
    {
        private UnitOfWork _unitOfWork;
        private readonly NorthwindEntities _context;
        private int DefaultPageSize = 10;

        public CustomersController()
        {
            _context = new NorthwindEntities();
            _unitOfWork = new UnitOfWork(_context);
        }

        // GET: Customers
        //public async Task<ActionResult> Index(string sortOrder, string searchString, string currentFilter, int? page, int? pageSize)
        public async Task<ActionResult> Index(CustomerIndexVM customerIndexVM)
        {
            // The null-coalescing operator defines a default value for a nullable type; 
            // the expression (page ?? 1) means return the value of page if it has a value 
            // or 1 if page is null.
            int pSize = (customerIndexVM.PageSize ?? DefaultPageSize);
            int pNumber = (customerIndexVM.Page ?? 1);
           

            // ASP.NET Session State Overview
            // https://msdn.microsoft.com/en-us/library/ms178581.aspx 
            string firstName = "Jeff";
            string lastName = "Smith";
            string city = "Seattle";

            // Save session variables
            //Session["FirstName"] = firstName;
            //Session["LastName"] = lastName;
            //Session["City"] = city;

            // Read session variables
            //firstName = (string)(Session["First"]);
            //lastName = (string)(Session["Last"]);
            //city = (string)(Session["City"]);

            //if (city == null)
            // No such value in session state; take appropriate action.

            // When retrieving an object from session state, cast it to 
            // the appropriate type.
            //ArrayList stockPicks = (ArrayList)Session["StockPicks"];

            // Write the modified stock picks list back to session state.
            //Session["StockPicks"] = stockPicks;


            // Provide the view with the current sort order.
            //customerIndexVM.CurrentSort = customerIndexVM.SortOrder ?? "";
            customerIndexVM.SortOrder = customerIndexVM.SortOrder ?? "";

            // toggle sort params for links in view.
            if (String.IsNullOrEmpty(customerIndexVM.SortOrder))
                customerIndexVM.CompanyNameSortParm = "company_name_desc";
            else
                customerIndexVM.CompanyNameSortParm = customerIndexVM.SortOrder == "company_name" ? "company_name_desc" : "company_name";

            customerIndexVM.ContactNameSortParm = customerIndexVM.SortOrder == "contact_name" ? "contact_name_desc" : "contact_name";

            // If the search string is changed during paging, the page has to be reset to 1, 
            // because the new filter can result in different data to display. 
            // The search string is changed when a value is entered in the text box and 
            // the submit button is pressed. In that case, the searchString parameter is not null.
            if (!String.IsNullOrEmpty(customerIndexVM.SearchString))
            {
                customerIndexVM.Page = 1;
            }
            else
            {
                customerIndexVM.SearchString = customerIndexVM.CurrentFilter;
            }

            // Provide the view with the current filter string. This value must be included 
            // in the paging links in order to maintain the filter settings during paging, 
            // and it must be restored to the textbox when the page is redisplayed.
            customerIndexVM.CurrentFilter = customerIndexVM.SearchString;

            customerIndexVM.Page = pNumber;
            customerIndexVM.PageSize = pSize;

            // var courses = _unitOfWork.CourseRepository.Get(includeProperties: "Department");
            //var customers = _unitOfWork.CustomerRepository.Get();
            var customers = from c in _context.Customers
                        orderby c.CompanyName
                        select c;

            if (!String.IsNullOrEmpty(customerIndexVM.SearchString))
            {
                customers = customers
                     .Where(c => c.CompanyName.Contains(customerIndexVM.SearchString))
                     .OrderBy(c => c.CompanyName);
            }

            switch (customerIndexVM.SortOrder)
            {
                case "company_name_desc":
                    customers = customers.OrderByDescending(s => s.CompanyName);
                    break;
                case "contact_name":
                    customers = customers.OrderBy(s => s.ContactName);
                    break;
                case "contact_name_desc":
                    customers = customers.OrderByDescending(s => s.ContactName);
                    break;
                default:
                    customers = customers.OrderBy(s => s.CompanyName);
                    break;
            }

            //return View(await db.Customers.ToListAsync());
            //return View(await customers.ToPagedListAsync(pNumber, pSize));
            customerIndexVM.CustomerPagedList = await customers.ToPagedListAsync(pNumber, pSize);
            return View(customerIndexVM);
        }

        // GET: Customers/Details/5
        public ActionResult Details(string id, int? selectedOrderID, int? page, int? pageSize)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Fetch customer from database
            //Customer customer = await _context.Customers.FindAsync(id);
            Customer customer = _unitOfWork.CustomerRepository.GetByID(id);

            // Populate Orders for selected Order
            if (selectedOrderID != null)
            {
                customer.Orders.Where(o => o.OrderID == selectedOrderID.GetValueOrDefault());
            }

            // Map entity to view model.
            var customerVM = Mapper.Map<CustomerVM>(customer);

            // Save selection
            customerVM.SelectedOrderID = selectedOrderID;

            // Populate OrderDetails for selected Order
            if (selectedOrderID != null)
            {
                customerVM.OrderDetails = _unitOfWork.OrderDetailRepository.Get().Where(o => o.OrderID == selectedOrderID).ToList();
                //customerVM.OrderDetails = _context.OrderDetails.Where(o => o.OrderID == selectedOrderID).ToList();
            }

            if (customer == null)
            {
                return HttpNotFound();
            }

            // Save current page settings
            customerVM.Page = page;
            customerVM.PageSize = pageSize;

            // Return strongly-typed view.
            return View(customerVM);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            var viewModel = new CustomerVM();
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] CustomerVM customerVM)
        {
            try
            {
                // Check for blank spaces entered by user
                if (customerVM.CustomerID.Trim().Length == 0)
                    ModelState.AddModelError("CustomerID", "CustomerID is required.");
                if (customerVM.CompanyName.Trim().Length == 0)
                    ModelState.AddModelError("CompanyName", "CompanyName is required.");

                // Check if CustomerID already exists.
                if (customerVM.CustomerID.Trim().Length != 0)
                {
                    //var customerAlreadyExists = _context.Customers.Where(s => s.CustomerID == customerVM.CustomerID).SingleOrDefault();
                    var customerAlreadyExists = _unitOfWork.CustomerRepository.GetByID(customerVM.CustomerID);
                    if (customerAlreadyExists != null)
                    {
                        ModelState.AddModelError("CustomerID", "CustomerID already exists.");
                        //ModelState.AddModelError(string.Empty, "CustomerID already exists.");
                    }
                }

                // Implicit model binding (using bind parameters)
                if (ModelState.IsValid)
                {
                    // Trim values entered by user
                    customerVM.CustomerID = customerVM.CustomerID.Trim();
                    customerVM.CompanyName = customerVM.CompanyName.Trim();
                    customerVM.ContactName = customerVM.ContactName.Trim();
                    customerVM.ContactTitle = customerVM.ContactTitle.Trim();

                    // Create new entity object and map values from view model
                    var newCustomer = Mapper.Map<Customer>(customerVM);

                    // Save to database
                    //_context.Customers.Add(newCustomer);
                    //await _context.SaveChangesAsync();
                    _unitOfWork.CustomerRepository.Insert(newCustomer);
                    _unitOfWork.Save();

                    return RedirectToAction("Index");
                }
            }
            //catch (DataException dex)
            catch (RetryLimitExceededException rex)
            {
                // Log the error
                // System.Reflection.MethodBase.GetCurrentMethod().ToString() will return entire method signature
                Logger.LogError(rex, "Error creating new record in " +
                    this.GetType().FullName + "." +
                    System.Reflection.MethodBase.GetCurrentMethod().Name);
                ModelState.AddModelError("", "Unable to create record. Try again, and if the problem persists see your system administrator.");
            }

            // Return strongly-typed view.
            return View(customerVM);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(string id, int? page, int? pageSize)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Using AutoMapper in Your ASP.Net MVC Applications
            // http://www.codeguru.com/columns/experts/using-automapper-in-your-asp.net-mvc-applications.htm

            // Fetch the customer from the database.
            //Customer customer = await _context.Customers.FindAsync(id);
            Customer customer = _unitOfWork.CustomerRepository.GetByID(id);

            // Map entity to view model.
            var customerVM = Mapper.Map<CustomerVM>(customer);

            if (customerVM == null)
            {
                return HttpNotFound();
            }

            // Save current page settings
            customerVM.Page = page;
            customerVM.PageSize = pageSize;

            // Return strongly-typed view.
            return View(customerVM);
        }


        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] CustomerVM customerVM)
        //public async Task<ActionResult> EditPost(string customerID, int? page, int? pageSize)
        {
            // Trim customerID
            customerVM.CustomerID = customerVM.CustomerID.Trim();

            // Check for missing customerID
            if (String.IsNullOrEmpty(customerVM.CustomerID))
            {
                ModelState.AddModelError("CustomerID", "CustomerID is required.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Trim values
                    customerVM.CompanyName = customerVM.CompanyName.Trim();
                    customerVM.ContactName = customerVM.ContactName.Trim();
                    customerVM.ContactTitle = customerVM.ContactTitle.Trim();

                    // Map view model to entity object
                    var customer = Mapper.Map<Customer>(customerVM);

                    // Save to database
                    //_context.Entry(customer).State = EntityState.Modified;
                    //await _context.SaveChangesAsync();
                    _unitOfWork.CustomerRepository.Update(customer);
                    _unitOfWork.Save();
                    
                    // Return to index page
                    return RedirectToAction("Index");
                }
                //catch (DataException dex)
                catch (RetryLimitExceededException rex)
                {
                    // Log the error 
                    Logger.LogError(rex, "Error saving record in " +
                        this.GetType().FullName + "." +
                        System.Reflection.MethodBase.GetCurrentMethod().Name);
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            return View(customerVM);
        }

/*
        //
        // Edit using TryUpdateModel().
        //
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CustomerID,CompanyName,ContactName,ContactTitle,Address,City,Region,PostalCode,Country,Phone,Fax")] CustomerVM customerVM)
        //public async Task<ActionResult> EditPost(string customerID, int? page, int? pageSize)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Entry(customer).State = EntityState.Modified;
            //    await db.SaveChangesAsync();
            //    return RedirectToAction("Index");
            //}
            //return View(customer);

            string[] fieldsToBind = new string[] { "CompanyName", "ContactName", "ContactTitle", "Address",
                                                    "City", "Region", "PostalCode", "Country", "Phone", "Fax" };

            Customer customer = await db.Customers.FindAsync(customerVM.CustomerID);

            if (customerVM.CustomerID.Trim().Length == 0)
                ModelState.AddModelError("CustomerID", "CustomerID is required.");

            if (ModelState.IsValid)
            {
                // explicit model binding
                if (TryUpdateModel(customer, fieldsToBind))
                {
                    try
                    {
                        customer.CompanyName = customer.CompanyName.Trim();
                        customer.ContactName = customer.ContactName.Trim();
                        customer.ContactTitle = customer.ContactTitle.Trim();
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                    //catch (DataException dex)
                    catch (RetryLimitExceededException rex)
                    {
                        // Log the error 
                        Logger.LogError(rex, "Error saving record in " +
                            this.GetType().FullName + "." +
                            System.Reflection.MethodBase.GetCurrentMethod().Name);
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
            }

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;

            return View(customer);
        }
*/

        // GET: Customers/Delete/5
        public async Task<ActionResult> Delete(string id, int? page, int? pageSize, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }

            Customer customer = await _context.Customers.FindAsync(id);
            var customerVM = Mapper.Map<CustomerVM>(customer);
            if (customerVM == null)
            {
                return HttpNotFound();
            }

            customerVM.Page = page;
            customerVM.PageSize = pageSize;

            return View(customerVM);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                //Customer customer = _context.Customers.FindAsync(id);
                //_context.Customers.Remove(customer);
                //_context.SaveChangesAsync();
                Customer customer = _unitOfWork.CustomerRepository.GetByID(id);
                _unitOfWork.CustomerRepository.Delete(id);
                _unitOfWork.Save();
            }
            //catch (DataException dex)
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
