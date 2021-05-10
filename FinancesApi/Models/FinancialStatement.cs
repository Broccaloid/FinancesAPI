using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class FinancialStatement
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public List<FinancialOperation> FinancialOperations { get; set; }
    }
}
