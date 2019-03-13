using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace LogisticsManagement_DataAccess
{
    public class EntityFrameworkGenericRepository<T> : IDataRepository<T> where T : class
    {
        private LogisticsContext _context;

        public EntityFrameworkGenericRepository(LogisticsContext context)
        {
            _context = context;
        }
       
        public void CallStoredProcedure(string procName, params Tuple<string, string>[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<T> GetAllList(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _context.Set<T>();

            foreach(var navProp in navigationProperties)
            {
                dbQuery = dbQuery.Include<T, object>(navProp);
            }

            var listData = dbQuery.ToList<T>();
            return listData;
        }

        public IList<T> GetFilteredList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _context.Set<T>();

            foreach (var navProp in navigationProperties)
            {
                dbQuery = dbQuery.Include<T, object>(navProp);
            }

            return dbQuery.Where(where).ToList<T>();
        }

        public T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _context.Set<T>();

            foreach (var navProp in navigationProperties)
            {
                dbQuery = dbQuery.Include<T, object>(navProp);
            }

            return dbQuery.Where(where).FirstOrDefault();
        }

        public void Add(params T[] items)
        {
            foreach (var item in items)
            {
                _context.Entry(item).State = EntityState.Added;
            }
            _context.SaveChanges();
        }

        public void Update(params T[] items)
        {
            foreach(var item in items)
            {
                _context.Entry(item).State = EntityState.Modified;
            }
            _context.SaveChanges();
        }

        public void Remove(params T[] items)
        {
            foreach (var item in items)
            {
                _context.Entry(item).State = EntityState.Deleted;
            }
            _context.SaveChanges();
        }

        
    }
}
