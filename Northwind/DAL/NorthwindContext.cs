using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Northwind.DAL
{
    public class NorthwindContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        // The name of the connection string (which you'll add to the Web.config file later) 
        // is passed in to the constructor. 
        // If you don't specify a connection string or the name of one explicitly, 
        // Entity Framework assumes that the connection string name is the same as the class name.
        public NorthwindContext() : base("name=NorthwindConnection")
        {
        }

        public DbSet<Northwind.Models.Employee> Employees { get; set; }
        public DbSet<Northwind.Models.Customer> Customers { get; set; }
    }
}
