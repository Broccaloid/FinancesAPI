using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class FinancialOperation
    {
        public int Id { get; set; }
        public int BalanceChange { get; set; }
        public string Type { get; set; }
        public string Date { get; set; }
    }
}
