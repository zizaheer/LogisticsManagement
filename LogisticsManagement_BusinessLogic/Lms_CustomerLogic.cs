using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_CustomerLogic : BaseLogic<Lms_CustomerPoco>
    {
        IMemoryCache _cache;
        public Lms_CustomerLogic(IMemoryCache cache, IDataRepository<Lms_CustomerPoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_CustomerPoco> GetList()
        {
            List<Lms_CustomerPoco> _customers;
            if(!_cache.TryGetValue(App_CacheKeys.Customers, out _customers))
            {
                _customers = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            return _customers;
        }

        public override List<Lms_CustomerPoco> GetListById(int id)
        {
            List<Lms_CustomerPoco> _customers;
            if (!_cache.TryGetValue(App_CacheKeys.Customers, out _customers))
            {
                _customers = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Customers, _customers, cacheEntryOptions);
            }

            return _customers;
        }

        public override Lms_CustomerPoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }


        #endregion

        #region Add/Update/Remove Methods

        public override Lms_CustomerPoco Add(Lms_CustomerPoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Customers);

            return addedPoco;
        }

        public override Lms_CustomerPoco Update(Lms_CustomerPoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Customers);

            return updatedPoco;
        }

        public override void Remove(Lms_CustomerPoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Add(Lms_CustomerPoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Update(Lms_CustomerPoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        public override void Remove(Lms_CustomerPoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Customers);
        }

        #endregion

        public string CreateNewCustomer(Lms_CustomerPoco customerPoco, Lms_AddressPoco billingAddressPoco, Lms_AddressPoco mailingAddressPoco, int branchId, bool isMailingBillingAddressSame = true )
        {
            if (JsonConvert.SerializeObject(billingAddressPoco) != JsonConvert.SerializeObject(mailingAddressPoco))
            {
                isMailingBillingAddressSame = false;
            }

            SqlParameter[] sqlParameters = {
                new SqlParameter("@CustomerName", SqlDbType.VarChar, 50) { Value = customerPoco.CustomerName },
                new SqlParameter("@BranchId", SqlDbType.Int) { Value = branchId },
                new SqlParameter("@IsGstApplicable", SqlDbType.Bit) { Value = customerPoco.IsGstApplicable },
                new SqlParameter("@DiscountPercentage", SqlDbType.Decimal) { Value = customerPoco.DiscountPercentage },
                new SqlParameter("@InvoiceDueDays", SqlDbType.Int) { Value = customerPoco.InvoiceDueDays },
                new SqlParameter("@MailingAddressId", SqlDbType.Int) { Value = customerPoco.MailingAddressId },
                new SqlParameter("@BillingAddressId", SqlDbType.Int) { Value = customerPoco.BillingAddressId },
                new SqlParameter("@IsMailingBillingAddressSame", SqlDbType.Bit) { Value = isMailingBillingAddressSame },
                new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = customerPoco.CreatedBy },

                new SqlParameter("@BillingUnitNo", SqlDbType.VarChar, 50) { Value = billingAddressPoco.UnitNumber },
                new SqlParameter("@BillingAddressLine", SqlDbType.VarChar, 150) { Value = billingAddressPoco.AddressLine },
                new SqlParameter("@BillingCityId", SqlDbType.Int) { Value = billingAddressPoco.CityId },
                new SqlParameter("@BillingProvinceId", SqlDbType.Int) { Value = billingAddressPoco.ProvinceId },
                new SqlParameter("@BillingCountryId", SqlDbType.Int) { Value = billingAddressPoco.CountryId },
                new SqlParameter("@BillingPostCode", SqlDbType.VarChar, 50) { Value = billingAddressPoco.PostCode },
                new SqlParameter("@BillingContactPersonName", SqlDbType.VarChar, 200) { Value = billingAddressPoco.ContactPersonName },
                new SqlParameter("@BillingPhoneNumber", SqlDbType.VarChar, 150) { Value = billingAddressPoco.PrimaryPhoneNumber },
                new SqlParameter("@BillingFaxNumber", SqlDbType.VarChar, 150) { Value = billingAddressPoco.Fax },
                new SqlParameter("@BillingEmailAddress", SqlDbType.VarChar, 200) { Value = billingAddressPoco.EmailAddress1 },

                new SqlParameter("@MailingUnitNo", SqlDbType.VarChar, 50) { Value = mailingAddressPoco.UnitNumber },
                new SqlParameter("@MailingAddressLine", SqlDbType.VarChar, 150) { Value = mailingAddressPoco.AddressLine },
                new SqlParameter("@MailingCityId", SqlDbType.Int) { Value = mailingAddressPoco.CityId },
                new SqlParameter("@MailingProvinceId", SqlDbType.Int) { Value = mailingAddressPoco.ProvinceId },
                new SqlParameter("@MailingCountryId", SqlDbType.Int) { Value = mailingAddressPoco.CountryId },
                new SqlParameter("@MailingPostCode", SqlDbType.VarChar, 50) { Value = mailingAddressPoco.PostCode },
                new SqlParameter("@MailingContactPersonName", SqlDbType.VarChar, 200) { Value = mailingAddressPoco.ContactPersonName },
                new SqlParameter("@MailingPhoneNumber", SqlDbType.VarChar, 150) { Value = mailingAddressPoco.PrimaryPhoneNumber },
                new SqlParameter("@MailingFaxNumber", SqlDbType.VarChar, 150) { Value = mailingAddressPoco.Fax },
                new SqlParameter("@MailingEmailAddress", SqlDbType.VarChar, 200) { Value = mailingAddressPoco.EmailAddress1 },
            };

            StringBuilder query = new StringBuilder();
            query.Append("EXEC CreateNewCustomer ");
            query.Append("@CustomerName, @BranchId, @IsGstApplicable, @DiscountPercentage, @InvoiceDueDays, @MailingAddressId, ");
            query.Append("@BillingAddressId, @IsMailingBillingAddressSame, @CreatedBy, ");

            query.Append("@BillingUnitNo, @BillingAddressLine, @BillingCityId, @BillingProvinceId, @BillingCountryId, @BillingPostCode, ");
            query.Append("@BillingContactPersonName, @BillingPhoneNumber, @BillingFaxNumber, @BillingEmailAddress, ");

            query.Append("@MailingUnitNo, @MailingAddressLine, @MailingCityId, @MailingProvinceId, @MailingCountryId, @MailingPostCode, ");
            query.Append("@MailingContactPersonName, @MailingPhoneNumber, @MailingFaxNumber, @MailingEmailAddress ");

            var outPut = base.CallStoredProcedure(query.ToString(), sqlParameters);

            return outPut;
        }
    }
}
