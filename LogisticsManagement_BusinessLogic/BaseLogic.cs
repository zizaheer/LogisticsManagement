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

        public virtual TPoco GetSingleById(int id)
        {
            return _repository.GetSingle(d => d.Id == id);
        }

        public virtual List<TPoco> GetList()
        {
            return _repository.GetList().ToList();
        }

        public virtual List<TPoco> GetListById(int id)
        {
            return _repository.GetList(d => d.Id == id).ToList();
        }

        public virtual TPoco Add(TPoco poco)
        {
            return _repository.Add(poco);
        }

        public virtual TPoco Update(TPoco poco)
        {
            return _repository.Update(poco);
        }

        public virtual void Remove(TPoco poco)
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
