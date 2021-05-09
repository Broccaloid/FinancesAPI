using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    interface IFinancialOperationRepository : IRepository<FinancialOperation>
    {
        IEnumerable<FinancialOperation> GetAllByDate(string date);
        IEnumerable<FinancialOperation> GetAllByDateRange(string dateStart, string dateEnd);
        IEnumerable<FinancialOperation> GetAllByType(string type);
    }
}
