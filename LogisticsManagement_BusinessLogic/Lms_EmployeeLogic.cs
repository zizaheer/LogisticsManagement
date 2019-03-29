using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeLogic : BaseLogic<Lms_EmployeePoco>
    {
        public Lms_EmployeeLogic(IDataRepository<Lms_EmployeePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeePoco Add(Lms_EmployeePoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_EmployeePoco Update(Lms_EmployeePoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
