using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi
{
    public static class DataSample
    {
        public static void SeedDb(FinancesContext financesContext)
        {
            if (!financesContext.FinancialOperations.Any())
            {
                financesContext.FinancialOperations.AddRange(
                    new FinancialOperation
                    {
                        Id = 0,
                        BalanceChange = -200,
                        Type = "money",
                        Date = new DateTime()
                    },
                    new FinancialOperation
                    {
                        Id = 1,
                        BalanceChange = 500,
                        Type = "moneyPlus",
                        Date = new DateTime(2001, 1, 12)
                    });
            }
            financesContext.SaveChanges();
        }
    }
}
