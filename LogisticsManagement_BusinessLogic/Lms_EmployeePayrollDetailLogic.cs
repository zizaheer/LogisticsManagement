using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeePayrollDetailLogic : BaseLogic<Lms_EmployeePayrollDetailPoco>
    {
        public Lms_EmployeePayrollDetailLogic(IDataRepository<Lms_EmployeePayrollDetailPoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeePayrollDetailPoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeePayrollDetailPoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeePayrollDetailPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeePayrollDetailPoco Add(Lms_EmployeePayrollDetailPoco poco)
        {
            return base.Add(poco);
        }

        public override Lms_EmployeePayrollDetailPoco Update(Lms_EmployeePayrollDetailPoco poco)
        {
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeePayrollDetailPoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeePayrollDetailPoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeePayrollDetailPoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeePayrollDetailPoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
