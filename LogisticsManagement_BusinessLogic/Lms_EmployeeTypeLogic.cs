using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeTypeLogic : BaseLogic<Lms_EmployeeTypePoco>
    {
        public Lms_EmployeeTypeLogic(IDataRepository<Lms_EmployeeTypePoco> repository) : base(repository)
        {
        }

        #region Get Methods

        public override List<Lms_EmployeeTypePoco> GetList()
        {
            return base.GetList();
        }

        public override List<Lms_EmployeeTypePoco> GetListById(int id)
        {
            return base.GetListById(id);
        }

        public override Lms_EmployeeTypePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeeTypePoco Add(Lms_EmployeeTypePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            return base.Add(poco);
        }

        public override Lms_EmployeeTypePoco Update(Lms_EmployeeTypePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            return base.Update(poco);
        }

        public override void Remove(Lms_EmployeeTypePoco poco)
        {
            base.Remove(poco);
        }

        public override void Add(Lms_EmployeeTypePoco[] pocos)
        {
            base.Add(pocos);
        }

        public override void Update(Lms_EmployeeTypePoco[] pocos)
        {
            base.Update(pocos);
        }

        public override void Remove(Lms_EmployeeTypePoco[] pocos)
        {
            base.Remove(pocos);
        }

        #endregion


    }
}
