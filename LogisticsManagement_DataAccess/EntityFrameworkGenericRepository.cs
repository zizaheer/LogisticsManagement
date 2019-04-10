using LogisticsManagement_Poco;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        public IList<T> GetList(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _context.Set<T>().AsNoTracking();

            foreach (var navProp in navigationProperties)
            {
                dbQuery = dbQuery.Include<T, object>(navProp);
            }

            var listData = dbQuery.ToList<T>();
            return listData;
        }

        public IList<T> GetList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _context.Set<T>().AsNoTracking();

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

        public int GetMaxId(Func<T, int> columnName)
        {
            var GetMaxId = _context.Set<T>().Max(columnName);
            return GetMaxId;
        }

        public T Add(T item)
        {
            _context.Entry(item).State = EntityState.Added;
            _context.SaveChanges();
            return item;
        }

        public T Update(T item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
            return item;
        }

        public void Remove(T item)
        {
            _context.Entry(item).State = EntityState.Deleted;
            _context.SaveChanges();
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
            foreach (var item in items)
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

        public string CallStoredProcedure(string query, params object[] parameters)
        {
            var type = _context.Query<Lms_StoredProcedureResult>().FromSql(query, parameters).ToList();

            return JsonConvert.SerializeObject(type.ToArray());
        }
    }
}
