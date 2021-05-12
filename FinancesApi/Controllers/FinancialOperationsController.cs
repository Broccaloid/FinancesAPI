using FinancesApi.Models;
using FinancesApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Controllers
{
    [Produces("application/json")]
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

        /// <summary>
        /// Returns all operations
        /// </summary>
        /// <returns>All operations</returns>
        /// <response code="200">Returns all operations</response>
        /// <resmonse code="404">If no operations were found</resmonse>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Returns all incomes
        /// </summary>
        /// <returns>All incomes</returns>
        /// <response code="200">Returns all incomes</response>
        /// <resmonse code="404">If no operations were found</resmonse>
        [HttpGet]
        [Route("incomes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Returns all expenses
        /// </summary>
        /// <returns>All expenses</returns>
        /// <response code="200">Returns all expenses</response>
        /// <resmonse code="404">If no operations were found</resmonse>
        [HttpGet]
        [Route("expenses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Returns a financial statement for a stated period of time
        /// </summary>
        /// <param name="dateStart"></param>
        /// <param name="dateEnd"></param>
        /// <returns>Financial statement</returns>
        /// <response code="200">Returns a financial statement</response>
        /// <response code="404">If no operations were found in a given time period</response>
        [HttpGet]
        [Route("finance")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Returns a daily financial statement
        /// </summary>
        /// <param name="date"></param>
        /// <returns>Daily financial statement</returns>
        /// <response code="200">Returns a daily financial statement</response>
        /// <response code="404">If no operations were found on a given day</response>
        [HttpGet]
        [Route("finance/daily")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Adds a list of operarations
        /// </summary>
        /// <param name="financialOperations"></param>
        /// <returns>Added operations</returns>
        /// <response code="200">Returns added operations</response>
        /// <response code="400">If no operations were sent</response>
        [HttpPost]
        [Route("operation/list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Adds an operation
        /// </summary>
        /// <param name="financialOperation"></param>
        /// <returns>Added operation</returns>
        /// <response code="200">Returns created operation</response>
        /// <response code="400">If no operation was sent</response>
        [HttpPost]
        [Route("operation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

        /// <summary>
        /// Changes an existing operation on a given one
        /// </summary>
        /// <param name="financialOperation"></param>
        /// <returns>Changed operation</returns>
        /// <response code="200">Returns changed operation</response>
        /// <response code="400">If no operation was sent</response>
        /// <response code="500">If a sent operation has an unknown id</response>
        [HttpPut]
        [Route("operation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<FinancialOperation> ChangeOperation([FromBody] FinancialOperation financialOperation)
        {
            if (financialOperation == null)
            {
                Logger.LogWarning("Incorrect request");
                return BadRequest("No values were sent");
            }
            return ChangeListOfOperation(new List<FinancialOperation>() { financialOperation});
        }

        /// <summary>
        /// Changes existing operations on given ones
        /// </summary>
        /// <param name="financialOperations"></param>
        /// <returns>Changed operations</returns>
        /// <response code="200">Returns changed operations</response>
        /// <response code="400">If no operations were sent</response>
        /// <response code="500">If sent operations have unknown ids</response>
        [HttpPut]
        [Route("operation/list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Deletes sent operation
        /// </summary>
        /// <param name="financialOperation"></param>
        /// <response code="204">If deleted successfully</response>
        /// <response code="400">If no operation was sent</response>
        /// <response code="500">If sent operation has an unknown id</response>
        [HttpDelete]
        [Route("operation")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Deletes sent operations
        /// </summary>
        /// <param name="financialOperations"></param>
        /// <response code="204">If deleted successfully</response>
        /// <response code="400">If no operations were sent</response>
        /// <response code="500">If sent operations have unknown ids</response>
        [HttpDelete]
        [Route("operation/list")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
