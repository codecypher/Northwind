using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Data.Entity.SqlServer;

// You can configure these settings manually for any database environment supported by an 
// Entity Framework provider, but default values that typically work well for an online 
// application that uses Windows Azure SQL Database have already been configured for you, 
// and those are the settings we implement.
//
// All we have to do to enable connection resiliency is create a class in our assembly 
// that derives from the DbConfiguration class, and in that class set the SQL Database 
// execution strategy, which in EF is another term for retry policy.
namespace Northwind.DAL
{
    // The Entity Framework automatically runs the code it finds in a class that derives 
    // from DbConfiguration. You can use the DbConfiguration class to do configuration 
    // tasks in code that you would otherwise do in the Web.config file.
    public class NorthwindConfiguration : DbConfiguration
    {
        public NorthwindConfiguration()
        {
            // Enable connection resiliency by setting the execution strategy or retry policy.
            // Change all of the catch blocks that catch DataException exceptions so that they 
            // catch RetryLimitExceededException exceptions instead.
            SetExecutionStrategy("System.Data.SqlClient", () => new SqlAzureExecutionStrategy());

            // Enabling command interception and logging
            //DbInterception.Add(new SchoolInterceptorTransientErrors());
            //DbInterception.Add(new SchoolInterceptorLogging());
        }
    }
}