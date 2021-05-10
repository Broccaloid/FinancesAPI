using FinancesApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Services
{
    public class StatementCalculator : IStatementCalculator
    {
        private readonly IUnitOfWork unitOfWork;

        public StatementCalculator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public FinancialStatement CaluculateStatement(DateTime dateStart, DateTime dateEnd)
        {
            decimal totalExpense = 0;
            decimal totalIncome = 0;
            var financialOperations = unitOfWork.FinancialOperations.GetAllByDateRange(dateStart, dateEnd).ToList();
            foreach (var operation in financialOperations)
            {
                if (operation.BalanceChange > 0)
                {
                    totalIncome += operation.BalanceChange;
                }
                else
                {
                    totalExpense += operation.BalanceChange;
                }
            }
            return new FinancialStatement
            {
                FinancialOperations = financialOperations,
                TotalExpense = totalExpense,
                TotalIncome = totalIncome
            };
        }
    }
}
