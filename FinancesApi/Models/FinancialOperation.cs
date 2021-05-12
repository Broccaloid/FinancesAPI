using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class FinancialOperation
    {
        public int Id { get; set; }
        [Required]
        public decimal BalanceChange { get; set; }
        [Required]
        public string Type { get; set; }
        [Column(TypeName = "date")]
        [Required]
        public DateTime Date { get; set; }
    }
}
