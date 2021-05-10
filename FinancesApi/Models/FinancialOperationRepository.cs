using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class FinancialOperationRepository : Repository<FinancialOperation>, IFinancialOperationRepository
    {
        public FinancesContext FinancesContext
        {
            get { return context as FinancesContext; }
        }

        public FinancialOperationRepository(FinancesContext context) : base(context) { }
        public IEnumerable<FinancialOperation> GetAllByDate(DateTime date)
        {
            return FinancesContext.FinancialOperations.Where(f => f.Date == date);
        }

        public IEnumerable<FinancialOperation> GetAllByDateRange(DateTime dateStart, DateTime dateEnd)
        {
            return FinancesContext.FinancialOperations.Where(f => f.Date >= dateStart && f.Date <= dateEnd);
        }

        public IEnumerable<FinancialOperation> GetAllByType(string type)
        {
            return FinancesContext.FinancialOperations.Where(f => f.Type == type);
        }
    }
}
