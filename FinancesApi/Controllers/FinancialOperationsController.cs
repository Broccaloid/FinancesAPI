using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FinancialOperationsController : ControllerBase
    {
        public IUnitOfWork UnitOfWork { get; }
        public IStatementCalculator StatementCalculator { get; }

        public FinancialOperationsController(IUnitOfWork unitOfWork, IStatementCalculator statementCalculator)
        {
            StatementCalculator = statementCalculator;
            UnitOfWork = unitOfWork;
        }

        [HttpGet]
        public IEnumerable<FinancialOperation> GetAllOperations()
        {
            return UnitOfWork.FinancialOperations.GetAll();
        }

        [HttpGet]
        [Route("allincomes")]
        public IEnumerable<FinancialOperation> GetAllIncomes()
        {
            return UnitOfWork.FinancialOperations.GetAll().Where(o => o.BalanceChange > 0);
        }

        [HttpGet]
        [Route("allexpenses")]
        public IEnumerable<FinancialOperation> GetAllExpenses()
        {
            return UnitOfWork.FinancialOperations.GetAll().Where(o => o.BalanceChange < 0);
        }

        [HttpGet]
        [Route("financialstatement")]
        public FinancialStatement GetFinancialStatementForTimePeriod(DateTime dateStart, DateTime dateEnd)
        {
            return StatementCalculator.CaluculateStatement(dateStart, dateEnd);
        }

        [HttpGet]
        [Route("dailyfinancialstatement")]
        public FinancialStatement GetDailyFinancialStatement(DateTime date)
        {
            return StatementCalculator.CaluculateStatement(date, date);
        }

        [HttpPost]
        [Route("addoperations")]
        public ActionResult<FinancialOperation> AddListOfOperations([FromBody] List<FinancialOperation> financialOperations)
        {
            foreach (var operation in financialOperations)
            {
                UnitOfWork.FinancialOperations.Add(operation);
            }
            UnitOfWork.Save();
            return Ok(financialOperations);
        }

        [HttpPost]
        [Route("addmanyoperation")]
        public ActionResult<FinancialOperation> AddOperation([FromBody] FinancialOperation financialOperation)
        {
            UnitOfWork.FinancialOperations.Add(financialOperation);
            UnitOfWork.Save();
            return Ok(financialOperation);
        }

        [HttpPut]
        [Route("changeoperation")]
        public ActionResult<FinancialOperation> ChangeOperation([FromBody] FinancialOperation financialOperation)
        {
            UnitOfWork.FinancialOperations.GetById(financialOperation.Id).BalanceChange = financialOperation.BalanceChange;
            UnitOfWork.FinancialOperations.GetById(financialOperation.Id).Date = financialOperation.Date;
            UnitOfWork.FinancialOperations.GetById(financialOperation.Id).Type = financialOperation.Type;
            UnitOfWork.Save();
            return Ok(financialOperation);
        }

        [HttpPut]
        [Route("changemanyoperations")]
        public ActionResult<FinancialOperation> ChangeListOfOperation([FromBody] List<FinancialOperation> financialOperations)
        {
            foreach (var operation in financialOperations)
            {
                UnitOfWork.FinancialOperations.GetById(operation.Id).BalanceChange = operation.BalanceChange;
                UnitOfWork.FinancialOperations.GetById(operation.Id).Date = operation.Date;
                UnitOfWork.FinancialOperations.GetById(operation.Id).Type = operation.Type;
            }
            UnitOfWork.Save();
            return Ok(financialOperations);
        }

        [HttpDelete]
        [Route("deleteoperation")]
        public void DeleteOperation([FromBody] FinancialOperation financialOperation)
        {
            UnitOfWork.FinancialOperations.Remove(financialOperation);
            UnitOfWork.Save();
        }

        [HttpDelete]
        [Route("deletemanyoperations")]
        public void DeleteListOfOperations([FromBody] List<FinancialOperation> financialOperations)
        {
            foreach (var operation in financialOperations)
            {
                UnitOfWork.FinancialOperations.Remove(operation);
            }
            UnitOfWork.Save();
        }
    }
}
