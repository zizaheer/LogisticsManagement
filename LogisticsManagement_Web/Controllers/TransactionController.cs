using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LogisticsManagement_BusinessLogic;
using LogisticsManagement_DataAccess;
using LogisticsManagement_Poco;
using LogisticsManagement_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace LogisticsManagement_Web.Controllers
{
    public class TransactionController : Controller
    {
        private Lms_TransactionDetailLogic _transactionDetailLogic;
        private Lms_TransactionLogic _transactionLogic;
        private Lms_ChartOfAccountLogic _chartOfAccountLogic;

        private readonly LogisticsContext _dbContext;
        IMemoryCache _cache;

        public TransactionController(IMemoryCache cache, LogisticsContext dbContext)
        {
            _cache = cache;
            _dbContext = dbContext;
            _transactionLogic = new Lms_TransactionLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionPoco>(_dbContext));
            _transactionDetailLogic = new Lms_TransactionDetailLogic(_cache, new EntityFrameworkGenericRepository<Lms_TransactionDetailPoco>(_dbContext));
            _chartOfAccountLogic = new Lms_ChartOfAccountLogic(_cache, new EntityFrameworkGenericRepository<Lms_ChartOfAccountPoco>(_dbContext));

        }

        public IActionResult Index()
        {
            var customerList = _transactionLogic.GetList();
            return View();
        }

        /// <summary>
        ///  Should be able to make single debit, multiple credit and single credit multiple debit entries. Therefore debit and credit info are passed as objects
        /// </summary>
        /// <param name="debitAccountInfo"></param>
        /// <param name="creditAccountInfo"></param>
        /// <param name="transactionAmount"></param>
        /// <param name="transactionDate"></param>
        /// <param name="valueDate"></param>
        /// <param name="transactionRemarks"></param>
        /// <returns></returns>
        public int MakeTransaction(List<TransactionModel> debitAccountInfo, List<TransactionModel> creditAccountInfo, decimal transactionAmount, DateTime transactionDate, DateTime valueDate, string transactionRemarks)
        {
            var transactionId = _transactionDetailLogic.GetMaxId() + 1;

            Lms_TransactionDetailPoco txnDetail = new Lms_TransactionDetailPoco();
            txnDetail.Id = transactionId;
            txnDetail.TransactionAmount = transactionAmount;
            txnDetail.TransactionDate = transactionDate;
            txnDetail.ValueDate = valueDate;
            txnDetail.Remarks = transactionRemarks;
            _transactionDetailLogic.Add(txnDetail);

            Lms_TransactionPoco _transactionPoco = new Lms_TransactionPoco();

            if (debitAccountInfo.Count == 1)
            {
                //Add Debit side
                _transactionPoco = new Lms_TransactionPoco();
                _transactionPoco.Id = transactionId;
                _transactionPoco.SerialNo = 1;
                _transactionPoco.AccountId = debitAccountInfo.FirstOrDefault().AccountId;
                _transactionPoco.TransactionAmount = debitAccountInfo.FirstOrDefault().TxnAmount;
                _transactionPoco.Remarks = transactionRemarks;
                if (_transactionPoco.TransactionAmount > 0)
                {
                    _transactionLogic.Add(_transactionPoco);
                    var debitAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                    debitAccount.CurrentBalance = debitAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                    _chartOfAccountLogic.Update(debitAccount);
                }

                //Add Credit side
                var serialNo = 2;
                for (int i = 0; i < creditAccountInfo.Count; i++)
                {
                    _transactionPoco = new Lms_TransactionPoco();
                    _transactionPoco.Id = transactionId;
                    _transactionPoco.SerialNo = serialNo + i;
                    _transactionPoco.AccountId = creditAccountInfo[i].AccountId;
                    _transactionPoco.TransactionAmount = (-1) * creditAccountInfo[i].TxnAmount;
                    _transactionPoco.Remarks = transactionRemarks;
                    if (_transactionPoco.TransactionAmount < 0)
                    {
                        _transactionLogic.Add(_transactionPoco);
                        var creditAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                        creditAccount.CurrentBalance = creditAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                        _chartOfAccountLogic.Update(creditAccount);
                    }
                }
            }

            else if (creditAccountInfo.Count == 1)
            {
                //Add Credit side
                _transactionPoco = new Lms_TransactionPoco();
                _transactionPoco.Id = transactionId;
                _transactionPoco.SerialNo = 1;
                _transactionPoco.AccountId = creditAccountInfo.FirstOrDefault().AccountId;
                _transactionPoco.TransactionAmount = (-1) * creditAccountInfo.FirstOrDefault().TxnAmount;
                _transactionPoco.Remarks = transactionRemarks;
                if (_transactionPoco.TransactionAmount < 0)
                {
                    _transactionLogic.Add(_transactionPoco);
                    var creditAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                    creditAccount.CurrentBalance = creditAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                    _chartOfAccountLogic.Update(creditAccount);
                }
                //Add Debit side
                var serialNo = 2;
                for (int i = 0; i < debitAccountInfo.Count; i++)
                {
                    _transactionPoco = new Lms_TransactionPoco();
                    _transactionPoco.Id = transactionId;
                    _transactionPoco.SerialNo = serialNo + i;
                    _transactionPoco.AccountId = debitAccountInfo[i].AccountId;
                    _transactionPoco.TransactionAmount = debitAccountInfo[i].TxnAmount;
                    _transactionPoco.Remarks = transactionRemarks;
                    if (_transactionPoco.TransactionAmount > 0)
                    {
                        _transactionLogic.Add(_transactionPoco);

                        var debitAccount = _chartOfAccountLogic.GetSingleById(_transactionPoco.AccountId);
                        debitAccount.CurrentBalance = debitAccount.CurrentBalance + _transactionPoco.TransactionAmount;
                        _chartOfAccountLogic.Update(debitAccount);
                    }
                }
            }

            return transactionId;
        }

        public void RemoveTransaction(int transactionId) {

            int debitAccountId = 0;
            int creditAccountId = 0;

            var tranDetailInfo = _transactionDetailLogic.GetSingleById(transactionId);
            if (tranDetailInfo != null)
            {
                _transactionDetailLogic.Remove(tranDetailInfo);

                var transInfo = _transactionLogic.GetList().Where(c => c.Id == transactionId).ToList();
                if (transInfo != null)
                {
                    var debitEntries = transInfo.Where(c => c.TransactionAmount > 0).ToList();
                    foreach (var entry in debitEntries) {
                        debitAccountId = entry.AccountId;
                        var debitAccountInfo = _chartOfAccountLogic.GetSingleById(debitAccountId);
                        debitAccountInfo.CurrentBalance = debitAccountInfo.CurrentBalance - entry.TransactionAmount;
                        _chartOfAccountLogic.Update(debitAccountInfo);

                        _transactionLogic.Remove(entry);
                    }

                    var creditEntries = transInfo.Where(c => c.TransactionAmount < 0).ToList();
                    foreach (var entry in creditEntries)
                    {
                        creditAccountId = entry.AccountId;
                        var creditAccountInfo = _chartOfAccountLogic.GetSingleById(creditAccountId);
                        creditAccountInfo.CurrentBalance = creditAccountInfo.CurrentBalance + (-1) * entry.TransactionAmount;
                        _chartOfAccountLogic.Update(creditAccountInfo);

                        _transactionLogic.Remove(entry);
                    }
                }
            }
        }

    }
}