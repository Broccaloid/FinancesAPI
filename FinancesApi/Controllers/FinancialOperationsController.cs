using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        public ILogger Logger { get; }

        public FinancialOperationsController(IUnitOfWork unitOfWork, IStatementCalculator statementCalculator, ILogger<FinancialOperationsController> logger)
        {
            Logger = logger;
            StatementCalculator = statementCalculator;
            UnitOfWork = unitOfWork;
        }

        [HttpGet]
        public ActionResult<IEnumerable<FinancialOperation>> GetAllOperations()
        {
            Logger.LogInformation("GetAllOperation was called");
            var result = UnitOfWork.FinancialOperations.GetAll();
            if (result.ToList().Count == 0)
            {
                Logger.LogWarning("No operations were found");
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("incomes")]
        public ActionResult<IEnumerable<FinancialOperation>> GetAllIncomes()
        {
            Logger.LogInformation("GetAllIncomes was called");
            var result = UnitOfWork.FinancialOperations.GetAll().Where(o => o.BalanceChange > 0);
            if (result.ToList().Count == 0)
            {
                Logger.LogWarning("No operations were found");
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("expenses")]
        public ActionResult<IEnumerable<FinancialOperation>> GetAllExpenses()
        {
            Logger.LogInformation("GetAllExpenses was called");
            var result = UnitOfWork.FinancialOperations.GetAll().Where(o => o.BalanceChange < 0);
            if (result.ToList().Count == 0)
            {
                Logger.LogWarning("No operations were found");
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("finance")]
        public ActionResult<FinancialStatement> GetFinancialStatementForTimePeriod(DateTime dateStart, DateTime dateEnd)
        {
            Logger.LogInformation("GetFinancialStatementForTimePeriod was called");
            var result = StatementCalculator.CaluculateStatement(dateStart, dateEnd);
            if (result.FinancialOperations.Count == 0)
            {
                Logger.LogWarning("No operations were found");
                return NotFound();
            }
            return Ok(result);
        }

        [HttpGet]
        [Route("finance/daily")]
        public ActionResult<FinancialStatement> GetDailyFinancialStatement(DateTime date)
        {
            Logger.LogInformation("GetDailyFinancialStatement was called");
            var result = StatementCalculator.CaluculateStatement(date, date);
            if (result.FinancialOperations.Count == 0)
            {
                Logger.LogWarning("No operations were found");
                return NotFound();
            }
            return Ok(result);
        }

        [HttpPost]
        [Route("operation/list")]
        public ActionResult<FinancialOperation> AddListOfOperations([FromBody] List<FinancialOperation> financialOperations)
        {
            if (financialOperations == null || financialOperations.Count == 0)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest("No values were sent");
            }
            UnitOfWork.FinancialOperations.AddRange(financialOperations);
            UnitOfWork.Save();
            return Ok(financialOperations);
        }

        [HttpPost]
        [Route("operation")]
        public ActionResult<FinancialOperation> AddOperation([FromBody] FinancialOperation financialOperation)
        {
            if (financialOperation == null)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest("No values were sent");
            }
            UnitOfWork.FinancialOperations.Add(financialOperation);
            UnitOfWork.Save();
            return Ok(financialOperation);
        }

        [HttpPut]
        [Route("operation")]
        public ActionResult<FinancialOperation> ChangeOperation([FromBody] FinancialOperation financialOperation)
        {
            if (financialOperation == null)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest("No values were sent");
            }
            return ChangeListOfOperation(new List<FinancialOperation>() { financialOperation});
        }

        [HttpPut]
        [Route("operation/list")]
        public ActionResult<FinancialOperation> ChangeListOfOperation([FromBody] List<FinancialOperation> financialOperations)
        {
            if (financialOperations == null || financialOperations.Count == 0)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest("No values were sent");
            }
            foreach (var operation in financialOperations)
            {
                try
                {
                    var operationForChange = UnitOfWork.FinancialOperations.GetById(operation.Id);
                    operationForChange.BalanceChange = operation.BalanceChange;
                    operationForChange.Date = operation.Date;
                    operationForChange.Type = operation.Type;
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error occurred");
                    return StatusCode(500);
                }
            }
            UnitOfWork.Save();
            return Ok(financialOperations);
        }

        [HttpDelete]
        [Route("operation")]
        public ActionResult DeleteOperation([FromBody] FinancialOperation financialOperation)
        {
            if (financialOperation == null)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest("No values were sent");
            }
            try
            {
                UnitOfWork.FinancialOperations.Remove(financialOperation);
                UnitOfWork.Save();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error occurred");
                return StatusCode(500);
            }
            return NoContent();
        }

        [HttpDelete]
        [Route("operation/list")]
        public ActionResult DeleteListOfOperations([FromBody] List<FinancialOperation> financialOperations)
        {
            if (financialOperations == null || financialOperations.Count == 0)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest("No values were sent");
            }
            try
            {
                UnitOfWork.FinancialOperations.RemoveRange(financialOperations);
                UnitOfWork.Save();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error occurred");
                return StatusCode(500);
            }
            return NoContent();
        }
    }
}
