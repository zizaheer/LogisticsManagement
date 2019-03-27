using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class BaseLogic<TPoco> where TPoco : IPoco
    {
        protected IDataRepository<TPoco> _repository;

        public BaseLogic(IDataRepository<TPoco> repository)
        {
            _repository = repository;
        }

        public virtual TPoco GetSinglePoco(int id)
        {
            return _repository.GetSingle(d => d.Id == id);
        }

        public virtual List<TPoco> GetAllList()
        {
            return _repository.GetAllList().ToList();
        }

        public virtual List<TPoco> GetFilteredList(int id)
        {
            return _repository.GetFilteredList(d => d.Id == id).ToList();
        }


        public virtual void AddSingle(TPoco poco)
        {
            _repository.Add(poco);
        }

        public virtual void UpdateSingle(TPoco poco)
        {
            _repository.Update(poco);
        }

        public virtual void RemoveSingle(TPoco poco)
        {
            _repository.Remove(poco);
        }

        public virtual void Add(TPoco[] pocos)
        {
            _repository.Add(pocos);
        }

        public virtual void Update(TPoco[] pocos)
        {
            _repository.Update(pocos);
        }

        public virtual void Remove(TPoco[] pocos)
        {
            _repository.Remove(pocos);
        }


    }
}
