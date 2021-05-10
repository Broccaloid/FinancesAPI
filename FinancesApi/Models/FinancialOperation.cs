using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class FinancialOperation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int BalanceChange { get; set; }
        public string Type { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
    }
}
