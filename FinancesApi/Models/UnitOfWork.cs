using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FinancesContext context;
        public IFinancialOperationRepository FinancialOperations { get; }

        public UnitOfWork(FinancesContext context)
        {
            this.context = context;
            FinancialOperations = new FinancialOperationRepository(context);
        }

        public void Save()
        {
            context.SaveChanges();
        }
    }
}
