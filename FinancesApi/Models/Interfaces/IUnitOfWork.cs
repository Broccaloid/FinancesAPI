using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public interface IUnitOfWork
    {
        IFinancialOperationRepository FinancialOperations { get; }

        public void Save();
    }
}
