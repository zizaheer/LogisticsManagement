using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace LogisticsManagement_DataAccess
{
    public interface IDataRepository<T>
    {
        T GetSingle(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);

        IList<T> GetFilteredList(Func<T,bool> where, params Expression<Func<T,object>>[] navigationProperties);

        IList<T> GetAllList(params Expression<Func<T,object>>[] navigationProperties);

        //void AddSingle(T item);

        //void UpdateSingle(T item);

        //void RemoveSingle(T item);

        void Add(params T[] items);

        void Update(params T[] items);

        void Remove(params T[] items);

        void CallStoredProcedure(string procName, params Tuple<string,string>[] parameters);
    }
}
