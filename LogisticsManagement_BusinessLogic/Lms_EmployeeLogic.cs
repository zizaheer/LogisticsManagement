using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;
using System.Data;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_EmployeeLogic : BaseLogic<Lms_EmployeePoco>
    {
        IMemoryCache _cache;
        public Lms_EmployeeLogic(IMemoryCache cache, IDataRepository<Lms_EmployeePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_EmployeePoco> GetList()
        {
            List<Lms_EmployeePoco> _customers;
            if (!_cache.TryGetValue(App_CacheKeys.Customers, out _customers))
            {
                _customers = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            return _customers;
        }

        public override List<Lms_EmployeePoco> GetListById(int id)
        {
            List<Lms_EmployeePoco> _customers;
            if (!_cache.TryGetValue(App_CacheKeys.Customers, out _customers))
            {
                _customers = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            return _customers;
        }

        public override Lms_EmployeePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_EmployeePoco Add(Lms_EmployeePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Customers);

            return addedPoco;
        }

        public override Lms_EmployeePoco Update(Lms_EmployeePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Customers);

            return updatedPoco;
        }

        public override void Remove(Lms_EmployeePoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Add(Lms_EmployeePoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Update(Lms_EmployeePoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Remove(Lms_EmployeePoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        #endregion

        public string CreateNewEmployee(Lms_EmployeePoco employeePoco, int branchId)
        {

            SqlParameter[] sqlParameters = {

                new SqlParameter("@FirstName", SqlDbType.VarChar, 50) { Value = employeePoco.FirstName },
                new SqlParameter("@LastName", SqlDbType.VarChar, 50) { Value = (object)employeePoco.LastName ?? DBNull.Value },
                new SqlParameter("@BranchId", SqlDbType.Int) { Value = branchId },
                new SqlParameter("@DriverLicenseNo", SqlDbType.VarChar, 50) { Value =(object) employeePoco.DriverLicenseNo ?? DBNull.Value },
                new SqlParameter("@SocialInsuranceNo", SqlDbType.VarChar, 50) { Value = (object)employeePoco.SocialInsuranceNo ?? DBNull.Value },
                new SqlParameter("@UnitNumber", SqlDbType.VarChar, 50) { Value = (object)employeePoco.UnitNumber ?? DBNull.Value },
                new SqlParameter("@HouseNumber", SqlDbType.VarChar, 50) { Value = (object)employeePoco.HouseNumber ?? DBNull.Value },
                new SqlParameter("@StreetNumber", SqlDbType.VarChar, 50) { Value =(object) employeePoco.StreetNumber ?? DBNull.Value },
                new SqlParameter("@AddressLine", SqlDbType.VarChar, 150) { Value =(object) employeePoco.AddressLine ?? DBNull.Value },
                new SqlParameter("@CityId", SqlDbType.Int) { Value = (object)employeePoco.CityId ?? DBNull.Value },
                new SqlParameter("@ProvinceId", SqlDbType.Int) { Value = (object)employeePoco.ProvinceId ?? DBNull.Value },
                new SqlParameter("@CountryId", SqlDbType.Int) { Value = (object)employeePoco.CountryId ?? DBNull.Value },
                new SqlParameter("@PostCode", SqlDbType.VarChar, 50) { Value = (object)employeePoco.PostCode ?? DBNull.Value },
                new SqlParameter("@PhoneNumber", SqlDbType.VarChar, 50) { Value = (object)employeePoco.PhoneNumber?? DBNull.Value  },
                new SqlParameter("@MobileNumber", SqlDbType.VarChar, 50) { Value = (object)employeePoco.MobileNumber?? DBNull.Value  },
                new SqlParameter("@FaxNumber", SqlDbType.VarChar, 50) { Value = (object)employeePoco.FaxNumber ?? DBNull.Value },
                new SqlParameter("@EmailAddress", SqlDbType.VarChar, 150) { Value = (object)employeePoco.EmailAddress ?? DBNull.Value },
                new SqlParameter("@EmployeeTypeId", SqlDbType.Int) { Value = (object)employeePoco.EmployeeTypeId?? DBNull.Value  },
                new SqlParameter("@IsHourlyPaid", SqlDbType.Bit) { Value = (object)employeePoco.IsHourlyPaid?? DBNull.Value  },
                new SqlParameter("@HourlyRate", SqlDbType.Decimal) { Value = (object)employeePoco.HourlyRate ?? DBNull.Value },
                new SqlParameter("@IsSalaried", SqlDbType.Bit, 50) { Value = (object)employeePoco.IsSalaried ?? DBNull.Value },
                new SqlParameter("@SalaryAmount", SqlDbType.Decimal) { Value = (object)employeePoco.SalaryAmount ?? DBNull.Value },
                new SqlParameter("@SalaryTerm", SqlDbType.Int) { Value = (object)employeePoco.SalaryTerm ?? DBNull.Value },
                new SqlParameter("@IsCommissionProvided", SqlDbType.Bit) { Value =(object) employeePoco.IsCommissionProvided ?? DBNull.Value },
                new SqlParameter("@CommissionPercentage", SqlDbType.Decimal) { Value = (object)employeePoco.CommissionPercentage ?? DBNull.Value },
                new SqlParameter("@IsFuelChargeProvided", SqlDbType.Bit) { Value = (object)employeePoco.IsFuelChargeProvided ?? DBNull.Value },
                new SqlParameter("@FuelPercentage", SqlDbType.Decimal) { Value = (object)employeePoco.FuelPercentage ?? DBNull.Value },
                new SqlParameter("@RadioInsuranceAmount", SqlDbType.Decimal) { Value =(object) employeePoco.RadioInsuranceAmount?? DBNull.Value  },
                new SqlParameter("@InsuranceAmount", SqlDbType.Decimal) { Value = (object)employeePoco.InsuranceAmount ?? DBNull.Value },
                new SqlParameter("@TermDays", SqlDbType.Int) { Value = (object)employeePoco.TermDays ?? DBNull.Value },
                new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = employeePoco.CreatedBy }

            };

            StringBuilder query = new StringBuilder();
            query.Append("EXEC CreateNewEmployee ");
            query.Append("@FirstName, @LastName, @BranchId, @DriverLicenseNo, @SocialInsuranceNo, @UnitNumber, ");
            query.Append("@HouseNumber, @StreetNumber, @AddressLine,  @CityId, @ProvinceId, @CountryId, ");

            query.Append("@PostCode, @PhoneNumber, @MobileNumber, @FaxNumber, @EmailAddress, @EmployeeTypeId, ");
            query.Append("@IsHourlyPaid, @HourlyRate, @IsSalaried, @SalaryAmount, ");

            query.Append("@SalaryTerm, @IsCommissionProvided, @CommissionPercentage, @IsFuelChargeProvided, @FuelPercentage, @RadioInsuranceAmount, ");
            query.Append("@InsuranceAmount, @TermDays, @CreatedBy ");

            var outPut = base.CallStoredProcedure(query.ToString(), sqlParameters);

            _cache.Remove(App_CacheKeys.Employees);
            _cache.Remove(App_CacheKeys.Accounts);

            return outPut;
        }
    }
}
