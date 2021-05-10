using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public interface IFinancialOperationRepository : IRepository<FinancialOperation>
    {
        IEnumerable<FinancialOperation> GetAllByDate(DateTime date);
        IEnumerable<FinancialOperation> GetAllByDateRange(DateTime dateStart, DateTime dateEnd);
        IEnumerable<FinancialOperation> GetAllByType(string type);
    }
}
