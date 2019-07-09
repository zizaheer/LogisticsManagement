using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class TransactionModel
    {
        public int AccountId { get; set; }
        public decimal TxnAmount { get; set; }
    }
}
