using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogisticsManagement_Web.Models
{
    public class ViewModel_EmployeeLoan
    {
        public int LoanId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public decimal LoanAmount { get; set; }
        public DateTime LoanTakenOn { get; set; }
        public string Remarks { get; set; }
    }
}
