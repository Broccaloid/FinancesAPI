using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class FinancesContext : DbContext
    {
        public DbSet<FinancialOperation> FinancialOperations { get; set; }
        public FinancesContext(DbContextOptions<FinancesContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
