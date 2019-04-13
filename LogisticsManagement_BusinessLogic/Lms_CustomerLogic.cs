using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using System.Linq;

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
                _customers = base.GetList().OrderBy(c => c.CustomerName).ToList();
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
                _customers = base.GetListById(id).OrderBy(c => c.CustomerName).ToList();
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
                new SqlParameter("@DiscountPercentage", SqlDbType.Decimal) { Value = (object)customerPoco.DiscountPercentage ?? DBNull.Value },
                new SqlParameter("@InvoiceDueDays", SqlDbType.Int) { Value = (object)customerPoco.InvoiceDueDays  ?? DBNull.Value},
                new SqlParameter("@MailingAddressId", SqlDbType.Int) { Value = (object)customerPoco.MailingAddressId  ?? DBNull.Value},
                new SqlParameter("@BillingAddressId", SqlDbType.Int) { Value = (object)customerPoco.BillingAddressId  ?? DBNull.Value},
                new SqlParameter("@IsMailingBillingAddressSame", SqlDbType.Bit) { Value = isMailingBillingAddressSame },
                new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = customerPoco.CreatedBy },

                new SqlParameter("@BillingUnitNo", SqlDbType.VarChar, 50) { Value = (object)billingAddressPoco.UnitNumber  ?? DBNull.Value},
                new SqlParameter("@BillingAddressLine", SqlDbType.VarChar, 150) { Value = (object)billingAddressPoco.AddressLine  ?? DBNull.Value},
                new SqlParameter("@BillingCityId", SqlDbType.Int) { Value = (object)billingAddressPoco.CityId  ?? DBNull.Value},
                new SqlParameter("@BillingProvinceId", SqlDbType.Int) { Value = (object)billingAddressPoco.ProvinceId  ?? DBNull.Value},
                new SqlParameter("@BillingCountryId", SqlDbType.Int) { Value = (object)billingAddressPoco.CountryId  ?? DBNull.Value},
                new SqlParameter("@BillingPostCode", SqlDbType.VarChar, 50) { Value = (object)billingAddressPoco.PostCode  ?? DBNull.Value},
                new SqlParameter("@BillingContactPersonName", SqlDbType.VarChar, 200) { Value = (object)billingAddressPoco.ContactPersonName  ?? DBNull.Value},
                new SqlParameter("@BillingPhoneNumber", SqlDbType.VarChar, 150) { Value = (object)billingAddressPoco.PrimaryPhoneNumber  ?? DBNull.Value},
                new SqlParameter("@BillingFaxNumber", SqlDbType.VarChar, 150) { Value = (object)billingAddressPoco.Fax  ?? DBNull.Value},
                new SqlParameter("@BillingEmailAddress", SqlDbType.VarChar, 200) { Value = (object)billingAddressPoco.EmailAddress1  ?? DBNull.Value},

                new SqlParameter("@MailingUnitNo", SqlDbType.VarChar, 50) { Value = (object)mailingAddressPoco.UnitNumber  ?? DBNull.Value },
                new SqlParameter("@MailingAddressLine", SqlDbType.VarChar, 150) { Value = (object)mailingAddressPoco.AddressLine  ?? DBNull.Value },
                new SqlParameter("@MailingCityId", SqlDbType.Int) { Value = (object)mailingAddressPoco.CityId  ?? DBNull.Value },
                new SqlParameter("@MailingProvinceId", SqlDbType.Int) { Value = (object)mailingAddressPoco.ProvinceId  ?? DBNull.Value },
                new SqlParameter("@MailingCountryId", SqlDbType.Int) { Value = (object)mailingAddressPoco.CountryId  ?? DBNull.Value },
                new SqlParameter("@MailingPostCode", SqlDbType.VarChar, 50) { Value = (object)mailingAddressPoco.PostCode  ?? DBNull.Value },
                new SqlParameter("@MailingContactPersonName", SqlDbType.VarChar, 200) { Value = (object)mailingAddressPoco.ContactPersonName  ?? DBNull.Value },
                new SqlParameter("@MailingPhoneNumber", SqlDbType.VarChar, 150) { Value = (object)mailingAddressPoco.PrimaryPhoneNumber  ?? DBNull.Value },
                new SqlParameter("@MailingFaxNumber", SqlDbType.VarChar, 150) { Value = (object)mailingAddressPoco.Fax  ?? DBNull.Value },
                new SqlParameter("@MailingEmailAddress", SqlDbType.VarChar, 200) { Value = (object)mailingAddressPoco.EmailAddress1  ?? DBNull.Value }
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

            _cache.Remove(App_CacheKeys.Customers);
            _cache.Remove(App_CacheKeys.Addresses);
            _cache.Remove(App_CacheKeys.Accounts);

            return outPut;
        }
    }
}
