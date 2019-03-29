using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeePayrollLogic : BaseLogic<Lms_EmployeePayrollPoco>
    {
        public Lms_EmployeePayrollLogic(IDataRepository<Lms_EmployeePayrollPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeePayrollPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeePayrollPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeePayrollPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeePayrollPoco Add(Lms_EmployeePayrollPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_EmployeePayrollPoco Update(Lms_EmployeePayrollPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeePayrollPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeePayrollPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeePayrollPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeePayrollPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
