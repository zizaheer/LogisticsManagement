using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogisticsManagement_DataAccess
{
    public interface IDataRepository<T>
    {
        T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);

        IList<T> GetList(Func<T,bool> where, params Expression<Func<T,object>>[] navigationProperties);

        IList<T> GetList(params Expression<Func<T,object>>[] navigationProperties);

        T Add(T item);

        T Update(T item);

        void Remove(T item);

        void Add(params T[] items);

        void Update(params T[] items);

        void Remove(params T[] items);

        void CallStoredProcedure(string procName, params Tuple<string,string>[] parameters);

        int GetMaxId(Func<T, int> columnName);
    }
}
