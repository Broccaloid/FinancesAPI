using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class FinancesContext : DbContext
    {
        public DbSet<FinancialOperation> FinancialOperations { get; set; }
        public FinancesContext(DbContextOptions<FinancesContext> options, IWebHostEnvironment env) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
