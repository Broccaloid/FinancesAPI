using FinancesApi.Models;
using System;

namespace FinancesApi.Services
{
    public interface IStatementCalculator
    {
        FinancialStatement CaluculateStatement(DateTime dateStart, DateTime dateEnd);
    }
}