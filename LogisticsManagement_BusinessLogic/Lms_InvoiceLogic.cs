using LogisticsManagement_Poco;
using LogisticsManagement_DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Data.SqlClient;

namespace LogisticsManagement_BusinessLogic
{
    public class Lms_InvoiceLogic : BaseLogic<Lms_InvoicePoco>
    {
        IMemoryCache _cache;
        public Lms_InvoiceLogic(IMemoryCache cache, IDataRepository<Lms_InvoicePoco> repository) : base(repository)
        {
            _cache = cache;
        }

        #region Get Methods

        public override List<Lms_InvoicePoco> GetList()
        {
            List<Lms_InvoicePoco> _invoices;
            if (!_cache.TryGetValue(App_CacheKeys.Invoices, out _invoices))
            {
                _invoices = base.GetList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Invoices, _invoices, cacheEntryOptions);
            }

            return _invoices;
        }

        public override List<Lms_InvoicePoco> GetListById(int id)
        {
            List<Lms_InvoicePoco> _invoices;
            if (!_cache.TryGetValue(App_CacheKeys.Invoices, out _invoices))
            {
                _invoices = base.GetListById(id);
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromDays(3));
                _cache.Set(App_CacheKeys.Invoices, _invoices, cacheEntryOptions);
            }

            return _invoices;
        }

        public override Lms_InvoicePoco GetSingleById(int id)
        {
            return base.GetSingleById(id);
        }

        public override int GetMaxId()
        {
            return base.GetMaxId();
        }

        public string GetPendingInvoiceOrders()
        {

            var outPut = base.CallStoredProcedure("EXEC GetPendingInvoiceOrders");
            return outPut;
        }



        #endregion

        #region Add/Update/Remove Methods

        public override Lms_InvoicePoco Add(Lms_InvoicePoco poco)
        {
            poco.CreateDate = DateTime.Now;

            var addedPoco = base.Add(poco);
            _cache.Remove(App_CacheKeys.Invoices);

            return addedPoco;
        }

        public override Lms_InvoicePoco Update(Lms_InvoicePoco poco)
        {
            poco.CreateDate = Convert.ToDateTime(poco.CreateDate);
            var updatedPoco = base.Update(poco);
            _cache.Remove(App_CacheKeys.Invoices);

            return updatedPoco;
        }

        public override void Remove(Lms_InvoicePoco poco)
        {
            base.Remove(poco);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Add(Lms_InvoicePoco[] pocos)
        {
            base.Add(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Update(Lms_InvoicePoco[] pocos)
        {
            base.Update(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        public override void Remove(Lms_InvoicePoco[] pocos)
        {
            base.Remove(pocos);
            _cache.Remove(App_CacheKeys.Invoices);
        }

        #endregion

        public string GenerateInvoice(int billerCustomerId, string billerDepartment, int createdBy, string[] wayBillNumbers)
        {
            try
            {
                string wbNumbers = "";

                DataTable wayBillNumberList = new DataTable();
                wayBillNumberList.Columns.Add("WayBillNumber", typeof(string));

                if (wayBillNumbers.Length > 0)
                {
                    //wbNumbers = wayBillNumbers[0].ToString();

                    foreach (var item in wayBillNumbers)
                    {
                        var row = wayBillNumberList.NewRow();
                        row["WayBillNumber"] = item;
                        wayBillNumberList.Rows.Add(row);

                        if (!wbNumbers.Contains(item))
                        {
                            wbNumbers = wbNumbers + item.ToString() + ", ";
                        }
                    }
                }

                wbNumbers = wbNumbers.Trim().TrimEnd(',');

                SqlParameter[] sqlParameters = {
                new SqlParameter("@BillerCustomerId", SqlDbType.Int) { Value = billerCustomerId },
                new SqlParameter("@BillerDepartment", SqlDbType.VarChar, 100) { Value = billerDepartment },
                new SqlParameter("@wbNumbers", SqlDbType.VarChar, 100) { Value = wbNumbers },
                new SqlParameter("@CreatedBy", SqlDbType.Int) { Value = createdBy },
                new SqlParameter("@WayBillNumberList", SqlDbType.Structured) { TypeName = "dbo.WayBillNumbers", Value = wayBillNumberList }
            };

                StringBuilder query = new StringBuilder();
                query.Append("EXEC GenerateInvoice @BillerCustomerId, @BillerDepartment, @wbNumbers, @CreatedBy, @WayBillNumberList ");

                var outPut = base.CallStoredProcedure(query.ToString(), sqlParameters);

                _cache.Remove(App_CacheKeys.Invoices);
                _cache.Remove(App_CacheKeys.InvoiceWayBillMappings);
                return outPut;
            }
            catch (Exception ex)
            {
                return "";
            }

        }
    }
}
