
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Northwind.Models;

namespace Northwind.DAL
{
    /// <summary>
    /// Implement a Generic Repository and a Unit of Work Class
    ///
    /// Implementing the Repository and Unit of Work Patterns in an ASP.NET MVC Application
    /// https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRepository<TEntity> where TEntity : class
    {
        private readonly NorthwindEntities _context;
        private DbSet<TEntity> _dbSet;

        // The constructor accepts a database context instance and 
        // initializes the entity set variable.
        public GenericRepository(NorthwindEntities context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        // The Get method uses lambda expressions to allow the calling code 
        // to specify a filter condition and a column to order the results by, 
        // and a string parameter lets the caller provide a comma-delimited 
        // list of navigation properties for eager loading.
        //
        // By using these parameters, you ensure that the work is done by the 
        // database rather than the web server.
        //
        // The caller will provide a lambda expression for filter based on the
        // TEntity type, and this expression will return a Boolean value,
        // e.g. student => student.LastName == "Smith".
        //
        // The caller will provide a lambda expression for orderBy.
        // The input to the expression is an IQueryable object for the TEntity type. 
        // The expression will return an ordered version of that IQueryable object,
        // e.g. q => q.OrderBy(s => s.LastName)
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            // Create an IQueryable object 
            IQueryable<TEntity> query = _dbSet;

            // Apply the filter expression to query if there is one
            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Applies the eager-loading expressions after parsing the comma-delimited list
            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            // Apply the orderBy expression if there is one and return the results.
            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }
    }
}

