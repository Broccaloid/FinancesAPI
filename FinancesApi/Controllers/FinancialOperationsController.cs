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
        [Route("allincomes")]
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
        [Route("allexpenses")]
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
        [Route("financialstatement")]
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
        [Route("dailyfinancialstatement")]
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
        [Route("addmanyoperations")]
        public ActionResult<FinancialOperation> AddListOfOperations([FromBody] List<FinancialOperation> financialOperations)
        {
            Logger.LogInformation("AddListOfOperations was called");
            if (financialOperations == null || financialOperations.Count == 0)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest();
            }
            UnitOfWork.FinancialOperations.AddRange(financialOperations);
            UnitOfWork.Save();
            return Ok(financialOperations);
        }

        [HttpPost]
        [Route("addoperation")]
        public ActionResult<FinancialOperation> AddOperation([FromBody] FinancialOperation financialOperation)
        {
            Logger.LogInformation("AddOperation was called");
            if (financialOperation == null)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest();
            }
            UnitOfWork.FinancialOperations.Add(financialOperation);
            UnitOfWork.Save();
            return Ok(financialOperation);
        }

        [HttpPut]
        [Route("changeoperation")]
        public ActionResult<FinancialOperation> ChangeOperation([FromBody] FinancialOperation financialOperation)
        {
            Logger.LogInformation("ChangeOperation was called");
            if (financialOperation == null)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest();
            }
            return ChangeListOfOperation(new List<FinancialOperation>() { financialOperation});
        }

        [HttpPut]
        [Route("changemanyoperations")]
        public ActionResult<FinancialOperation> ChangeListOfOperation([FromBody] List<FinancialOperation> financialOperations)
        {
            Logger.LogInformation("ChangeListOfOperation was called");
            if (financialOperations == null || financialOperations.Count == 0)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest();
            }
            foreach (var operation in financialOperations)
            {
                try
                {
                    UnitOfWork.FinancialOperations.GetById(operation.Id).BalanceChange = operation.BalanceChange;
                    UnitOfWork.FinancialOperations.GetById(operation.Id).Date = operation.Date;
                    UnitOfWork.FinancialOperations.GetById(operation.Id).Type = operation.Type;
                }
                catch (Exception ex)
                {
                    Logger.LogWarning(ex, "Error occurred");
                    return NotFound(operation);
                }
            }
            UnitOfWork.Save();
            return Ok(financialOperations);
        }

        [HttpDelete]
        [Route("deleteoperation")]
        public ActionResult DeleteOperation([FromBody] FinancialOperation financialOperation)
        {
            Logger.LogInformation("DeleteOperation was called");
            if (financialOperation == null)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest();
            }
            try
            {
                UnitOfWork.FinancialOperations.Remove(financialOperation);
                UnitOfWork.Save();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error occurred");
            }
            return NoContent();
        }

        [HttpDelete]
        [Route("deletemanyoperations")]
        public ActionResult DeleteListOfOperations([FromBody] List<FinancialOperation> financialOperations)
        {
            Logger.LogInformation("DeleteListOfOperations was called");
            if (financialOperations == null || financialOperations.Count == 0)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest();
            }
            try
            {
                UnitOfWork.FinancialOperations.RemoveRange(financialOperations);
                UnitOfWork.Save();
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Error occurred");
            }
            return NoContent();
        }
    }
}
